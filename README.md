# Ballance TAS Editor

A editor written for TAS file editing.

## Basic Interface

### Program Menu

* File
    * Open: Open a TAS file.
    * Save: Save modifications to opended TAS file.
    * Save as...: Save the modifications as a new TAS file and switch to the new file.
    * Close: Close current file.
* Edit
    * Undo: Undo a operation.
    * Redo: Redo previous undo operation.
    * Item Count: Set the count of items showed in editor panel.
    * Overwritten Paste: Switch paste mode between Overwritten Mode and Insert Mode.
    * Horizontal Layout: Switch editor layout between horzontal layout and vertical layout.
* Help
    * Report Bugs: Open a web page to report bugs about this program.
    * About: Open a dialog to show some infomations about this prrogram.

### TAS Unit Menu

Right click TAS unit will open a menu. The enable statue of each is decided by your current tools and selected items.

|Operation|Needed mode|Needed selection|What the operation do|
|:---|:---|:---|:---|
|Set|Fill mode|Multiple selection|Set selected units as *set* status|
|Unset|Fill mode|Multiple selection|Set selected units as *unset* status|
|Cut|Cursor mode|Multiple selection|Cut selected units|
|Copy|Cursor mode|Multiple selection|Copy selected units|
|Paste after this|Cursor mode|Single selection|Pasted copied units after selected unit|
|Paste before this|Cursor mode|Single selection|Pasted copied units before selected unit|
|Delete|Cursor mode|Single selection|Delete selected units|
|Delete this frame|Cursor mode|Single selection|Delete selected unit, and move selection to the next unit|
|Delete last frame|Cursor mode|Single selection|Delete one unit aheading of selected unit|
|Add blank item after this|Cursor mode|Single selection|Add blank unit after selected unit|
|Add blank item before this|Cursor mode|Single selection|Add blank unit before selected unit|

### Status Bar

A status bar will be shown at the bottom of window if you opended a TAS file. The item located in status bar from left to right is: Current tools mode, Overwritten paste status, Current selected region.

## View

### Basic Operation

Once a file opened, a slider will be placed at the bottom of window. You can drag it and go to any position of this file which you want to browse.  
4 buttons were located at the left of slider, from left to right is: fast rewind, rewind, forward, fast forward. Fase backward and fast forward will rewind or forward the number of units by one page at a time.

If you want to change the count of shown units in display panel, please use Edit - Item Count to change it.

### Quick Operation

The key ASDF of keyboard are corresponding with the functions of 4 buttons.

A mouse wheel will scroll 1 TAS unit.
Or, if you press Shift at the same time, it will scroll 1 page TAS units.
Or, if you press Crtl at the same time, it will increase or decrease the count of shown item in display panel.

### Horzontal Layout and Vertical Layout

After opening a file, you can use Edit - Horizontal Layout to switch layout between horzontal layout and vertical layout. Different layouts suit for different people. Please choose your favorite layout freely.

## Mode Introduction

### Basic Mode

Once ths file opened, 3 tools buttons were placed at the top of window. They are:

* Cursor mode: Allow you pick TAS unit by column. In this mode, you can copy, paste, insert, delete and etc...
* Fill mode: Allow you select TAS unit by cell. And you can set or unset the status of cells.
* Draw mode: See cursor as brush. Flip the set status of clicked cell.

### Multiple and Single Selection

Cursor mode allow single selection. Click a TAS unit directly, you can select it.

Cursor mode and Fill mode allow multiple selection. For a multiple selection, you should click a cell as the start point, then hold on shift to click another cell, as the end of selection.  
Click a cell again will start a new selection, hold shift click a cell to finish the new multiple selection.  
If you want to multiple select a single cell(if operation require), you can hold on shift to click the start of selection to finish a single cell's multiple selection.

### Highlight of Selection

In cursor mode, selected TAS unit will show a orange rectangle at the top of them.

In fill mode, selected cell will show with orange border.

## Special Operation

### Overwritten Paste and Insert Paste

Paste have 2 mode, overwritten paste and insert paste.  
Insert will insert copied content before or after selected unit.
However, overwrite pasting will use the currently selected cell as the beginning or end of the paste, and write the contents of the clipboard forward or backward directly. The original data of affected unit will be wipe out. If the length of data is not enough, additional items will be added. 

### Shortcut

* `Ctrl + O`：Open file
* `Ctrl + S`：Save file
* `Ctrl + Z`：Undo
* `Ctrl + Y`：Redo
* `Ctrl + X`：Cut
* `Ctrl + C`：Copy
* `Ctrl + V`：Paste After
* `Delete`：Delete this frame
* `Backspace`：Delete last frame

## Something Went Wrong

If program crashed, program will output a error log in the folder called `logs`. Please send error log to developer to help us to locate and reproduce error.

