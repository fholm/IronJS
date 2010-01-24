# IronJS

IronJS is a ECMAScript 3.0 implementation built on top of the [Dynamic Language Runtime](http://dlr.codeplex.com/) from [Microsoft](http://www.microsoft.com/) which allows you to embed a javascript runtime into yor .NET applications.

## License

IronJS is released under the [GNU General Public License v3](http://www.gnu.org/licenses/gpl-3.0.html).

## State

The current state of the code is at best alpha quality. Use at your own risk.

## Features


### Types

Section 8 of the ECMA Script 3.0 specification

<table>

	<thead>
		<tr>
			<th>Section</th>	
			<th>Name</th>
			<th>Status</th>
			<th>Note</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>8.1</td>
			<td><strong>Undefined</strong></td>
			<td>Done</td>
			<td><em>Singleton instance IronJS.Runtime.Js.Undefined.Instance</em></td>
		</tr>
		<tr>
			<td>8.2</td>
			<td><strong>Null</strong></td>
			<td>Done</td>
			<td><em>Uses built in null in .NET</em></td>
		</tr>
		<tr>
			<td>8.3</td>
			<td><strong>Boolean</strong></td>
			<td>Done</td>
			<td><em>Uses built in boolean in .NET</em></td>
		</tr>
		<tr>
			<td>8.4</td>
			<td><strong>String</strong></td>
			<td>Done</td>
			<td><em>Uses built in string in .NET</em></td>
		</tr>
		<tr>
			<td>8.5</td>
			<td><strong>Number</strong></td>
			<td>Done</td>
			<td><em>Uses built in double in .NET</em></td>
		</tr>
		<tr>
			<td>8.6</td>
			<td><strong>Object</strong></td>
			<td>Done</td>
			<td><em>Interface in IronJS.Runtime.Js.IObj, base impl. in IronJS.Runtime.Js.Obj</em></td>
		</tr>
	</tbody>
</table>

### Type Conversions

Section 9 of the ECMA 3.0 specification

<table>

	<thead>
		<tr>
			<th>Section</th>	
			<th>Name</th>
			<th>Status</th>
			<th>Note</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>9.1</td>
			<td><strong>ToPrimitive</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
		<tr>
			<td>9.2</td>
			<td><strong>ToBoolean</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
		<tr>
			<td>9.3</td>
			<td><strong>ToNumber</strong></td>
			<td>Done</td>
			<td><em>Uses the Convert.ToDouble from the BCL for strings</em></td>
		</tr>
		<tr>
			<td>9.4</td>
			<td><strong>ToInteger</strong></td>
			<td>Done</td>
			<td><em>Uses the Convert.ToInt32 from the BCL for strings</em></td>
		</tr>
		<tr>
			<td>9.5</td>
			<td><strong>ToInt32</strong></td>
			<td>Done</td>
			<td><em>Uses the Convert.ToInt32 from the BCL for strings</em></td>
		</tr>
		<tr>
			<td>9.6</td>
			<td><strong>ToUInt32</strong></td>
			<td>Done</td>
			<td><em>Uses the Convert.ToUInt32 from the BCL for strings</em></td>
		</tr>
		<tr>
			<td>9.7</td>
			<td><strong>ToUint16</strong></td>
			<td>Done</td>
			<td><em>Uses the Convert.ToUint16 from the BCL for strings</em></td>
		</tr>
		<tr>
			<td>9.8</td>
			<td><strong>ToString</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
		<tr>
			<td>9.10</td>
			<td><strong>ToObject</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
	</tbody>
</table>

### Expressions

Section 11 of the ECMA 3.0 specification.

<table>

	<thead>
		<tr>
			<th>Section</th>	
			<th>Name</th>
			<th>Status</th>
			<th>Note</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>11.1.1</td>
			<td><strong>this keyword</strong></td>
			<td>Done</td>
			<td><em>this</em></td>
		</tr>
		<tr>
			<td>11.1.2</td>
			<td><strong>Identifier reference</strong></td>
			<td>Done</td>
			<td><em>x</em></td>
		</tr>
		<tr>
			<td>11.1.3</td>
			<td><strong>Literal reference</strong></td>
			<td>Done</td>
			<td><em>'x'</em></td>
		</tr>
		<tr>
			<td>11.1.4</td>
			<td><strong>Array initialiser</strong></td>
			<td>Done</td>
			<td><em>[ ]</em></td>
		</tr>
		<tr>
			<td>11.1.5</td>
			<td><strong>Object initializer</strong></td>
			<td>Done</td>
			<td><em>{ }</em></td>
		</tr>
		<tr>
			<td>11.1.6</td>
			<td><strong>Grouping operator</strong></td>
			<td>Done</td>
			<td><em>( )</em></td>
		</tr>
		<tr>
			<td>11.2.1</td>
			<td><strong>Property accessors</strong></td>
			<td>Done</td>
			<td><em>x.y</em></td>
		</tr>
		<tr>
			<td>11.2.2</td>
			<td><strong>New operator</strong></td>
			<td>Done</td>
			<td><em>new x()</em></td>
		</tr>
		<tr>
			<td>11.2.3</td>
			<td><strong>Function calls</strong></td>
			<td>Done</td>
			<td><em>x()</em></td>
		</tr>
		<tr>
			<td>11.2.4</td>
			<td><strong>Arguments list</strong></td>
			<td>Done</td>
			<td><em>x(1, 2, 3)</em></td>
		</tr>
		<tr>
			<td>11.2.5</td>
			<td><strong>Function expression</strong></td>
			<td>Done</td>
			<td><em>function() { }</em></td>
		</tr>
		<tr>
			<td>11.3.1</td>
			<td><strong>Postfix increment operator</strong></td>
			<td>Done</td>
			<td><em>x++</em></td>
		</tr>
		<tr>
			<td>11.3.2</td>
			<td><strong>Postfix decrement operator</strong></td>
			<td>Done</td>
			<td><em>x--</em></td>
		</tr>
		<tr>
			<td>11.4.1</td>
			<td><strong>delete operator</strong></td>
			<td>90%</td>
			<td><em>delete x (returns null instead of true/false)</em></td>
		</tr>
		<tr>
			<td>11.4.2</td>
			<td><strong>void operator</strong></td>
			<td>Done</td>
			<td><em>void x</em></td>
		</tr>
		<tr>
			<td>11.4.3</td>
			<td><strong>typeof operator</strong></td>
			<td>Done</td>
			<td><em>typeof x</em></td>
		</tr>
		<tr>
			<td>11.4.4</td>
			<td><strong>Prefix increment operator</strong></td>
			<td>Done</td>
			<td><em>++x</em></td>
		</tr>
		<tr>
			<td>11.4.5</td>
			<td><strong>Prefix decrement operator</strong></td>
			<td>Done</td>
			<td><em>--x</em></td>
		</tr>
		<tr>
			<td>11.4.6</td>
			<td><strong>Unary + operator</strong></td>
			<td>Done</td>
			<td><em>+x</em></td>
		</tr>
		<tr>
			<td>11.4.7</td>
			<td><strong>Unary - operator</strong></td>
			<td>Done</td>
			<td><em>-x</em></td>
		</tr>
		<tr>
			<td>11.4.8</td>
			<td><strong>Bitwise NOT operator</strong></td>
			<td>Done</td>
			<td><em>~x</em></td>
		</tr>
		<tr>
			<td>11.4.9</td>
			<td><strong>Logical NOT operator</strong></td>
			<td>Done</td>
			<td><em>!x</em></td>
		</tr>
		<tr>
			<td>11.5.1</td>
			<td><strong>* operator</strong></td>
			<td>Done</td>
			<td><em>x*y</em></td>
		</tr>
		<tr>
			<td>11.5.2</td>
			<td><strong>/ operator</strong></td>
			<td>Done</td>
			<td><em>x/y</em></td>
		</tr>
		<tr>
			<td>11.5.3</td>
			<td><strong>% operator</strong></td>
			<td>Done</td>
			<td><em>x%y</em></td>
		</tr>
		<tr>
			<td>11.6.1</td>
			<td><strong>+ operator</strong></td>
			<td>Done</td>
			<td><em>x+y</em></td>
		</tr>
		<tr>
			<td>11.6.2</td>
			<td><strong>- operator</strong></td>
			<td>Done</td>
			<td><em>x-y</em></td>
		</tr>
		<tr>
			<td>11.7.1</td>
			<td><strong>&lt;&lt; operator</strong></td>
			<td>Done</td>
			<td><em>x&lt;&lt;y</em></td>
		</tr>
		<tr>
			<td>11.7.2</td>
			<td><strong>&gt;&gt; operator</strong></td>
			<td>Done</td>
			<td><em>x&gt;&gt;y</em></td>
		</tr>
		<tr>
			<td>11.7.3</td>
			<td><strong>&gt;&gt;&gt; operator</strong></td>
			<td>Done</td>
			<td><em>x&gt;&gt;&gt;y</em></td>
		</tr>
		<tr>
			<td>11.8.1</td>
			<td><strong>&lt; operator</strong></td>
			<td>Done</td>
			<td><em>x&lt;y</em></td>
		</tr>
		<tr>
			<td>11.8.2</td>
			<td><strong>&gt; operator</strong></td>
			<td>Done</td>
			<td><em>x&gt;y</em></td>
		</tr>
		<tr>
			<td>11.8.3</td>
			<td><strong>&lt;= operator</strong></td>
			<td>Done</td>
			<td><em>x&lt;=y</em></td>
		</tr>
		<tr>
			<td>11.8.4</td>
			<td><strong>&gt;= operator</strong></td>
			<td>Done</td>
			<td><em>x&gt;=y</em></td>
		</tr>
		<tr>
			<td>11.8.6</td>
			<td><strong>instanceof operator</strong></td>
			<td>Done</td>
			<td><em>x instanceof y</em></td>
		</tr>
		<tr>
			<td>11.8.7</td>
			<td><strong>in operator</strong></td>
			<td>Done</td>
			<td><em>'x' in y</em></td>
		</tr>
		<tr>
			<td>11.9.1</td>
			<td><strong>== operator</strong></td>
			<td>Done</td>
			<td><em>x == y</em></td>
		</tr>
		<tr>
			<td>11.9.2</td>
			<td><strong>!= operator</strong></td>
			<td>Done</td>
			<td><em>x != y</em></td>
		</tr>
		<tr>
			<td>11.9.4</td>
			<td><strong>=== operator</strong></td>
			<td>Done</td>
			<td><em>x === y</em></td>
		</tr>
		<tr>
			<td>11.9.5</td>
			<td><strong>!== operator</strong></td>
			<td>Done</td>
			<td><em>x !== y</em></td>
		</tr>
		<tr>
			<td>11.10</td>
			<td><strong>Binary bitwise operators</strong></td>
			<td>Done</td>
			<td><em>x&y x|y x^y</em></td>
		</tr>
		<tr>
			<td>11.11</td>
			<td><strong>Logical bitwise operators</strong></td>
			<td>Done</td>
			<td><em>x&&y x||y</em></td>
		</tr>
		<tr>
			<td>11.12</td>
			<td><strong>Conditional operator</strong></td>
			<td>Done</td>
			<td><em>x?y:z</em></td>
		</tr>
		<tr>
			<td>11.13.1</td>
			<td><strong>Simple assignment</strong></td>
			<td>Done</td>
			<td><em>x=y</em></td>
		</tr>
		<tr>
			<td>11.13.2</td>
			<td><strong>Compound assignment</strong></td>
			<td>Done</td>
			<td><em>x@=y</em></td>
		</tr>
		<tr>
			<td>11.14</td>
			<td><strong>Comma operator</strong></td>
			<td>Done</td>
			<td><em>x,y</em></td>
		</tr>
	</tbody>
</table>

### Statements

Section 12 of the ECMA 3.0 specification.

<table>

	<thead>
		<tr>
			<th>Section</th>	
			<th>Name</th>
			<th>Status</th>
			<th>Note</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>12.1</td>
			<td><strong>Block</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.2</td>
			<td><strong>Variable</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.3</td>
			<td><strong>Empty</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.4</td>
			<td><strong>Expression</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.5</td>
			<td><strong>if</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.6.1</td>
			<td><strong>do-while</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.6.2</td>
			<td><strong>while</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.6.3</td>
			<td><strong>for</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.6.4</td>
			<td><strong>for-in</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.7</td>
			<td><strong>continue</strong></td>
			<td>Done</td>
		</tr>
		<tr>
			<td>12.8</td>
			<td><strong>break</strong></td>
			<td>90%</td>
			<td><em>Labelled break only works for a few statements</em></td>
		</tr>
		<tr>
			<td>12.9</td>
			<td><strong>return</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
		<tr>
			<td>12.10</td>
			<td><strong>with</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
		<tr>
			<td>12.11</td>
			<td><strong>switch</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
		<tr>
			<td>12.12</td>
			<td><strong>Labelled</strong></td>
			<td>90%</td>
			<td><em>Only a few statements are labelable</em></td>
		</tr>
		<tr>
			<td>12.13</td>
			<td><strong>throw</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
		<tr>
			<td>12.14</td>
			<td><strong>try</strong></td>
			<td>Done</td>
			<td><em></em></td>
		</tr>
	</tbody>
</table>