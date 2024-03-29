# AAI-Script-Editor-Europe-Edition
The GUI tool for editing Ace Attorney Investigations Miles Edgeworth script for Android.

# How to use this tool?
It's a GUI tool, so you need to press File-Open to open the extracted script file from AAI - Android. This tool has a previewer that shows how the script will look in the game.

# How do I extract scripts?
All AAI1's script is stored inside the game's code. You need to open game's Assembly-CSharp.dll file inside apk (or obb for the current build) with dnSpy in assets/bin/Data/Managed folder. Then, you need to open the class that contains the script, press "Edit class/method" button and copy all the text from opened window to some text file on your PC. Then you can open this text file with AAI Script Editor and it will translate all the bytes in arrays to a full working script.

# In which classes the script is stored?
GKJ1_MSG0 - the first case's script.<br>
GKJ1_MSG1 - the second case's script.<br>
GKJ1_MSG2 - the third case's script.<br>
GKJ1_MSG3 - the fourth case's script.<br>
GKJ1_MSG4 - the fifth case's script.<br>
GKJ1 - all Game Over and Partner scripts. I highly recommend you to edit not the GKJ1 class itself, but its static constructor because all the script is initialized in it.<br>
LogicMode - all Logic reasoning script during all the cases.<br>
ItemListMenu.ReadOnlyData - all item and some tutorial script is stored here. Once again, I reccomend you to edit only a static constructor.

# Anything to know about the code?
Yeah, it contains some remnants of my Russian edition of this editor, that you can download with this link: https://drive.google.com/file/d/1oWDs0EjPMeM8Gaq0wFvabbdIRZIMIpjS/view?usp=sharing<br>
Also, the program uses the characters that are available on this version of the game: https://drive.google.com/file/d/1x5t_K1rvFqAvnmQz0sO9xi-d6JVEbjJp/view?usp=drive_link<br>
Oh, and also the program contains a bug of crashing when there's any tag after end line tag in the same line, so please be careful with endline tags.

# Edit Menu doesn't work properly!
Yeah, I know. I couldn't add normal undo/redo functions and DS centering function can be applied only in Russian Edition. (since it contains information about Russian Characters)<br>
<b>BUT</b> you can use Centering button to adapt all bgm: 7 tags for your text. (bgm:7 - are tags used for centering. The last parameter means padding from the start.)
