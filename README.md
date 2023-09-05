# MultipleFilesRename
WPF program, designed to simplify the task of renaming multiple files and folders with precision and ease. This versatile application leverages C#, Regex, design patterns (Singleton, Factory, Abstract Factory, Prototype), a plugin architecture with DLL library files, and Delegate & Event handling to provide a seamless and efficient renaming experience. Say goodbye to manual renaming hassles and embrace a powerful tool that streamlines your file and folder management.

All existing rules in this program:
1. Change the extension to another extension (no conversion, force renaming extension)
2. Add counter to the end of the file
    - [ ]  Could specify the start value, steps, number of digits (Could have padding like 01, 02, 03...10....99)
3. Remove all space from the beginning and the ending of the filename
4. Replace certain characters into one character like replacing "-" ad "_" into space " "
    - [ ]  Could be the other way like replace all space " " into dot "."
5. Adding a prefix to all the files
6. Adding a suffix to all the files
7. Convert all characters to lowercase, remove all spaces
8. Convert filename to PascalCase
9. Remove original name

Besides that, it has some features:
1. You can drag the files / folders from File Explorer and drop those to the file list directly.
2. Adding recursively: just specify a folder only, the application will automatically scan and add all the files inside.
3. Checking exceptions when editing rules, when click batching to make sure that the files and folders all have valid name.
4. See the preview of the results.
5. Drag and drop between rules to change position.
