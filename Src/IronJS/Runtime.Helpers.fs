namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Runtime

[<AbstractClass>]
type Helpers =
  static member BuildClosureScopes (closure:Closure, evalObject, localScopes, scopeLevel) =
    new Scope(localScopes, evalObject, scopeLevel) :: closure.Scopes

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
      let cacheKey = throwToggle :: wasFoundToggle :: obj.ClassId :: classIds

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
                (Cache.buildCondition obj' (obj.ClassId :: classIds))
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
      x.ClassId <- -1 //This makes sure we will hit the crawler next time
      
      x.Crawler <- crawler //Save crawler
      x.Crawler.Invoke(x, obj, env) //Use crawler to get result