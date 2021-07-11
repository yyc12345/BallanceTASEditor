# Ballance TAS 编辑器

一款专门用于编辑Ballance TAS文件的编辑器

## 基本操作

### 菜单

* File
  * Open：打开一个TAS文件
  * Save：保存当前改动到TAS文件
  * Save As...：将当前改动保存到另一个新地方，保存完毕后文件自动切换成新目标，之后的保存操作将针对新文件来保存
  * Close：关闭当前文件
* Display
  * Item Count：设置一行显示多少个TAS操作单元：最少5个，最多30个
* Help
  * Report bugs：打开一个网页来汇报这个程序的Bug
  * About：关于此程序

### 状态栏

在打开文件后，底部状态栏将显示当前的状态：当前工具模式 和 当前选定的区域

## 移动视图

打开文件后，靠近底部有一个滑条，可以快速滑动到希望浏览的位置。  
滑条左侧有4个按钮，分别是：快退，退一个单元，进一个单元，快进。快进和快退将一次性前进或后退一个页面的单元数量。  
键盘上的ASDF四个键从左至右也分别对应滑条左侧的四个按钮的功能。

## 模式介绍

### 基本模式

打开文件后，上部3个带有图标的按钮，是TAS编辑器的三种模式，它们分别是：

* Select mode：选择模式：允许成列的选择TAS操作单元。在此模式下可以进行复制，粘贴，插入和删除操作。
* Fill mode：填充模式：像编辑表格那样，允许多列选择和跨行选择（但必须连续），并且支持在选择区域内批量设置或不设置项目
* Draw mode：绘画模式：将鼠标视为画笔，反转点击的单元格的设置状态。

### 如何单选与多选

选择模式允许单选，直接单击某一个TAS操作单元，即可选中。

选择模式和填充模式允许多选，具体操作是点击某个单元格作为起始位置，然后按住Shift点击第二个单元格作为终止位置。完成多选。  
再次左键单击重新开始一次选择。再次按住Shift左键单击重新选定终止位置。

### 选择的标识

选择模式下，选中的成列的TAS操作单元，其顶部的方框将呈现橘色。

填充模式下，选中的单元格的外边框将从灰色变为橘色。

### 模式中的菜单

在表格页面右键将打开一个操作菜单，如下：

* Set：位于填充模式且已多选时，设置选中单元为设置状态
* Unset：位于填充模式且已多选时，设置选中单元为不设置状态
* Copy：位于选择模式且已多选时，复制选中单元
* Delete：位于选择模式且已多选时，删除选中单元
* Paste after this：位于选择模式且已单选时，在其前方粘贴剪贴板中已复制的单元
* Paste before this：位于选择模式且已单选时，在其后方粘贴剪贴板中已复制的单元
* Add blank item after this：位于选择模式且已单选时，在其前方添加空白单元
* Add blank item before this：位于选择模式且已单选时，在其后方添加空白单元

## 出错啦

如果程序报告出错，会在程序所在文件夹下方的logs中生成错误报告，请将错误报告发送给开发者以方便定位错误。