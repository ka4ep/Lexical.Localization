# Writing Class Library
This tutorial shows how to use localization in class libraries.

Create new .NET Standard class library **TutorialLibrary**.

![create class library](img16.png)

There are three ways to add localization to class library.

## Method A: Static Instance
Static instance **LocalizationRoot.Global** can be used in class libraries for localization
when the class library doesn't need to support Inversion of Control (IoC) and Dependency Injection (DI).

Features:
* Allows inlining strings in the code
* Inlined strings can be scanned from the code
* Dependency to NuGet **Lexical.Localization**
* Application must setup static asset with initialized strings.
* Not for Inversion of Control

First, Add NuGet reference to **Lexical.Localization**.

Then, write this example class **MyController1.cs** that uses the static instance **LocalizationRoot.Global**.

```C#
using Lexical.Localization;
using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyController1
    {
        // Use this reference 
        static IAssetKey localization = LocalizationRoot.Global.Type(typeof(MyController1));

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
```

Localization strings can now be provided in the startup of the application by adding IAssets to the static instance.

```C#
using Lexical.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using TutorialLibrary;

namespace TutorialTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create asset
            Dictionary<string, string> strs = new Dictionary<string, string>();
            strs["fi:TutorialLibrary.MyController1:OK"] = "Toiminto onnistui";
            IAsset asset = new LocalizationStringAsset(strs, AssetKeyNameProvider.Default);

            // Add asset to global singleton instance
            LocalizationRoot.Builder.AddSource(asset);
            LocalizationRoot.Builder.Build();

            // Call Controller
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            MyController1 controller1 = new MyController1();
            Console.WriteLine(controller1.Do());
        }
    }
}
```

## Method B: Lexical.Localization.Abstractions
If class library supports Inversion of Control (IoC), then **Lexical.Localization.Abstractions** can be used for localization.

Features:
* Allows inlining strings in the code
* Inlined strings can be scanned from the code
* Dependency to NuGet **Lexical.Localization.Abstractions**
* Application must setup asset that is initialized with strings.
* For Inversion of Control

First, add NuGet reference to **Lexical.Localization.Abstractions**.

Then write this example code **MyController2.cs**

```C#
using Lexical.Localization;

namespace TutorialLibrary
{
    public class MyController2
    {
        IAssetKey localization;

        public MyController2(IAssetRoot root)
        {
            this.localization = root.Type(GetType());
        }

        public MyController2(IAssetKey<MyController2> localization)
        {
            this.localization = localization;
        }

        public string Do()
        {
            return localization.Key("OK").Inline("Operation Successful").ToString();
        }
    }
}
```

In the setup of the application IAssetRoot needs to be provided for the class.

```c#
using Lexical.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using TutorialLibrary;

namespace TutorialTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create asset
            Dictionary<string, string> strs = new Dictionary<string, string>();
            strs["fi:TutorialLibrary.MyController2:OK"] = "Toiminto onnistui";
            IAsset asset = new LocalizationStringAsset(strs, AssetKeyNameProvider.Default);

            // Create asset root
            IAssetRoot root = new LocalizationRoot(asset, new CulturePolicy());

            // Call Controller
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            MyController2 controller2 = new MyController2(root); 
            Console.WriteLine( controller2.Do() );
        }
    }
}
```


## Method C: Microsoft.Extensions.Localization.Abstractions

Features:
* Doesn't allow inlining strings in the code
* Keys of strings can be scanned from the code, but not inlined values
* Dependency to NuGet **Microsoft.Extensions.Localization.Abstractions**
* Application must setup asset that is initialized with strings.
* Usable for Inversion of Control

First, add NuGet reference to **Microsoft.Extensions.Localization.Abstractions**.

Then write this example code **MyController3.cs**

```C#
using Microsoft.Extensions.Localization;

namespace TutorialLibrary
{
    public class MyController3
    {
        IStringLocalizer localization;

        public MyController3(IStringLocalizer<MyController3> localization)
        {
            this.localization = localization;
        }

        public MyController3(IStringLocalizerFactory localizationFactory)
        {
            this.localization = localizationFactory.Create(GetType());
        }

        public string Do()
        {
            return localization["OK"].ToString();
        }
    }

}
```

In the setup of the application IStringLocalizerFactory or IStringLocalizer&lt;Type&gt; needs to be provided for the class.

```c#
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using TutorialLibrary;

namespace TutorialTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create asset
            Dictionary<string, string> strs = new Dictionary<string, string>();
            strs["fi:TutorialLibrary.MyController3:OK"] = "Toiminto onnistui";
            IAsset asset = new LocalizationStringAsset(strs, AssetKeyNameProvider.Default);

            // Create asset root
            IStringLocalizerFactory root = new StringLocalizerRoot(asset, new CulturePolicy());

            // Call Controller
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            MyController3 controller3 = new MyController3(root); 
            Console.WriteLine( controller3.Do() );
        }
    }
}
```
