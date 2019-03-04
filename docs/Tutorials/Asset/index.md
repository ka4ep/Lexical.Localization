## Localization Asset
This tutorial shows how to load language strings from an external file.
<br/>

Add new text file **"localization.ini"** into the C# project.

![Add new](img6.png)

![new .ini](img10.png)

<br/>
Paste the following text to the **localization.ini**, and then save the document.

[!code-ini[ini](../../localization.ini)]

<br/>
Go to properties of **localization.ini** and change **Copy to Output Directory** to **Copy always**. Now the file will be copied to the .exe folder.

![Copy always](img11.png)

<br/>
Next, open the **Program.cs** and modify the code to the following.

[!code-csharp[snippet](Example.cs)]

Now run the program.

![Hallo Welt](img12.png)
