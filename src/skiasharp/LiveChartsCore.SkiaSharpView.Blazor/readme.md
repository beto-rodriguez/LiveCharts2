# About *.ts

If the domInterop.ts changes, the *.js files must be manually updated, this due a possible bug where the
*.js files were not consistently generated on the MSBuild process, by the Microsoft.TypeScript.MSBuild package.

I am not completely sure if this is a bug on the MSBuild, the Microsoft.TypeScript.MSBuild the or
if it is a bug on the way I am using it.

Since the domInterop.ts barely changes, let's keep it simple and update the *.js files manually.

Calling `tsc` on the terminal will generate the *.js files, typescript must be installed globally,
`npm install -g typescript`.
