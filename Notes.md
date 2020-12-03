## Generic project structure in F#

```
-- XXXTypes (e.g. DomainTypes)
   // Contains types that are spread among multiple modules

-- Example
      |_ lib
      |   |
      |   |_Example.FirstModule.fs
      |       namespace Exmple
      |
      |       // declare all needed types
      |
      |       module FirstModule =
      |         // open all needed dependencies
      |         // define all operations on declared types
      |   ...
      |   |
      |   |_Example.LastModule.fs
            // every module in a separate file
      |
      |_ Example.Implemenation.fs
      |  // main logic of that module
      |
      |_ Readme.md
      |
      |_ Makefile
           build:
           test:
           repl:

-- Example.Test

-- Library1
....
-- LibraryN

-- Implementation.Types
-- Implementation (main functionality)

-- Project.sln
-- Readme.md
-- LICENSE
-- .gitignore
-- .gitattributes
```
