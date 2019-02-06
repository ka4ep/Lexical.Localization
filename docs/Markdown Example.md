---
uid: DocumentGlobalId
---
Link [Cheat Sheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet).
Link [DocFX Flavored Markdown](https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html?tabs=tabid-1%2Ctabid-a).

Reference Link [1]

![text](images/logo.svg)

*Emphasis*, aka italice, aka <cite>cite</cite>

**Strong emphasis**, bold, double asterisks or __double underscore__.

~~StrikeThrough~~

Paragraph separated by newlines.

[//]: # (comment line here)

# Header 1

## Header 2

### Header 3

#### Header 4

Header 1
========

Header 2
--------

This list list:
1. First item
2. Second Item
  * Unordered Item
  * Another item
100. 100th item

Another list uses minuses (-)
- hei
- hoi

This list uses plusses (+)
+ hei
+ hoi

### Inlined code
```C#
List<int> myList = new List<int>();
```

### Tables

| Column1  | Column2 | Column3 |
|----------|:-------:|--------:|
| Hello    | center  | right   |


[1]: http://www.google.fi
[logo]: https://github.com/adam-p/markdown-here/raw/master/src/common/images/icon48.png

### Collapsible

<details>
  <summary>Your header here! (<u>Click to expand</u>)</summary>
  Your content here...
  > markup like blockquote's should even work on github!
  more content here...
</details>


> [!TIP]
> <note content>
> [!NOTE]
> <note content>
> [!WARNING]
> <warning content>


# Introductions

Refer to [Markdown](http://daringfireball.net/projects/markdown/) for how to write markdown files.
## Quick Start Notes:
1. Add images to the *images* folder if the file is referencing an image.

## Collapse Test
<details>
  <summary>Your header here! (<u>Click to expand</u>)</summary>
  Your content here...
  > markup like blockquote's should even work on github!
  more content here...
</details>


## Note Test
Note Test
> [!NOTE]
> This is my important note!

Warning Test
> [!WARNING]
> Danger Will Robinson.

Tip Test
> [!TIP]
> Live it.



## Tab Group Test
Tab group 1:

# [Tab Text 1](#tab/tabid-1)

Tab content-1-1.

# [Tab Text 2](#tab/tabid-2)

Tab content-2-1.

***

Tab group 2:

# [Tab Text A](#tab/tabid-a)

Tab content-a-1.

# [Tab Text B](#tab/tabid-b)

Tab content-b-1.

***

Tab group 3:

# [Tab Text 1](#tab/tabid-1)

Tab content-1-1.

# [Tab Text 2](#tab/tabid-2)

Tab content-2-1.

***

Tab group 4:

# [Tab Text A](#tab/tabid-a)

Tab content-a-2.

# [Tab Text B](#tab/tabid-b)

Tab content-b-2.

