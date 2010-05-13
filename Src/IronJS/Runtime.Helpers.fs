namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Runtime

module Cache = 

  let classIdEq expr n =
    Expr.eq (Expr.field expr "ClassId") (Expr.constant n)
  
  let crawlPrototypeChain expr n = 
    Seq.fold (fun s _ -> Expr.field expr "Prototype") expr (seq{0..n-1})

  let buildCondition object' classIds = 
    (Expr.andChain
      (List.mapi 
        (fun i x -> 
          let prototype = crawlPrototypeChain object' i
          (Expr.and' (Expr.notDefault prototype) (classIdEq prototype x))
        )
        (classIds)
      )
    )

[<AbstractClass>]
type DynamicScope =
  static member GetGlobalVariable (name:string, globals:Object, scopes:Object list, closure:Closure) =
    ()

  static member SetGlobalVariable (name:string, value:Box, globals:Object, scopes:Object list, closure:Closure) =
    ()

[<AbstractClass>]
type Helpers =
  static member BuildClosureScopes (closure:Closure, localScopes, scopeLevel) =
    new Scope(localScopes, scopeLevel) :: closure.Scopes

  //This function handles the updating of
  //the cache cell in case of a miss
  static member UpdateGetCache (x:GetCache, obj:Object, env:Environment) =
    let box = obj.Get(x, env)

    //We found what we were looking for
    if x.ClassId = obj.ClassId then
      box

    //If not...
    else
      //Check if a prototype has it
      let index, classIds = obj.PrototypeHas x.Name

      //The constants -4 and -2 here could be anything
      //They are used to differ types of configurations
      //from eachother in the env.GetCrawlers Dict key
      let throwToggle = if x.ThrowOnMissing then -4 else -2
      let wasFoundToggle = if index < 0 then -4 else -2

      //Build key and try to find an already cached crawler
      let cacheKey = throwToggle :: wasFoundToggle :: obj.MapId :: classIds

      let crawler = 
        match Map.tryFind cacheKey env.GetCrawlers with
        | Some(cached) -> cached
        | None -> 
          //Parameters
          let x' = Expr.paramT<GetCache> "~x"
          let obj' = Expr.paramT<Object> "~obj"
          let env' = Expr.paramT<Environment> "~env"

          //Body differs
          //depending on if...
          let body = 
            if index >= 0 then
              //... we found the property
              (Expr.access 
                (Expr.field (Cache.crawlPrototypeChain obj' (classIds.Length)) "Properties") 
                [Expr.field x' "index"]
              )
            else
              //... or not
              (Expr.field env' "UndefinedBox")

          //Build lambda expression
          let lambda = 
            (Expr.lambdaT<GetCrawler> 
              [x'; obj'; env']
              (Expr.ternary 
                (Cache.buildCondition obj' (obj.MapId :: classIds))
                //If condition holds, execute body
                (body)
                //If condition fails, update
                (Expr.callStaticT<Helpers> "UpdateGetCache" [x'; obj'; env'])
              )
            )

          //Compile and to add it to cache
          env.GetCrawlers <- Map.add cacheKey (lambda.Compile()) env.GetCrawlers
          env.GetCrawlers.[cacheKey]

      //Setup cache to be ready for next hit
      x.Index   <- index //Save index so we know which offset to look at
      x.MapId <- -1 //This makes sure we will hit the crawler next time
      
      x.Crawler <- crawler //Save crawler
      x.Crawler.Invoke(x, obj, env) //Use crawler to get result

  static member UpdateSetCache (x:SetCache, obj:Object, value:Box byref, env:Environment) =
    //First try to do a normal update
    obj.Update(x, ref value)

    //And if we don't succeed
    if x.ClassId <> obj.ClassId then
      
      //Check if a prototype has it
      let index, classIds = obj.PrototypeHas x.Name

      //If we didn't find it in a Prototype
      //means we should create it on our current object
      if index < 0 then 
        obj.Create(x, ref value, env)

      //If we actually did find it, we need to
      //create a crawler that can set the property
      //for us in the future
      else
        //Build key and try to find an already cached crawler
        let cacheKey = obj.ClassId :: classIds

        let crawler =
          match Map.tryFind cacheKey env.SetCrawlers with
          | Some(cached) -> cached
          | None ->
            //Parameters
            let x' = Expr.paramT<SetCache> "~x"
            let obj' = Expr.paramT<Object> "~obj"
            let value' = Expr.param "~value" (typeof<Box>.MakeByRefType()) 
            let env' = Expr.paramT<Environment> "~env"

            //Build lambda expression
            let lambda = 
              (Expr.lambdaT<SetCrawler> 
                [x'; obj'; value'; env']
                (Expr.ternary 
                  //Condition: Object + All Prototypes must
                  //not be null and have matching ClassIds
                  (Cache.buildCondition obj' (obj.ClassId :: classIds))

                  //If condition holds, execute body
                  (Expr.castVoid
                    (Expr.assign
                      (Expr.access 
                        (Expr.field (Cache.crawlPrototypeChain obj' (classIds.Length)) "Properties") 
                        [Expr.field x' "index"]
                      )
                      (value')
                    )
                  )

                  //If condition fails, update
                  (Expr.callStaticT<Helpers> "UpdateSetCache" [x'; obj'; value'; env'])
                )
              )

            //Compile and to add it to cache
            env.SetCrawlers <- Map.add cacheKey (lambda.Compile()) env.SetCrawlers
            env.SetCrawlers.[cacheKey]

        //Setup cache to be ready for next hit
        x.Index   <- index
        x.ClassId <- -1
      
        x.Crawler <- crawler
        x.Crawler.Invoke(x, obj, ref value, env)
