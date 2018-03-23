| [Back]() |

---

# SetTemplateDefaultOptions
***v1.3.2+***
## Overloads
|   |    | 
| :--- | :--- | 
| **SetTemplateDefaultOptions()** | Sets the default options for image templates | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *True* | 
| **Return Value** | *array* |

---

### Description
Sets the default template options for any function utilizing the AnalyzeScreen class(`CheckScreen()`,`ImageLocation()`). The array can 7 items in the order of `Threshold, Reduction, Split, CustomX, CustomY, CustomWidth, CustomHeight`. If you wish to not edit a value, enter `null` as the value.

Defaults:

 * Threshold = 90
 * Reduction = 20
 * Split = "All"
 * CustomX - CustomHeight = 0

Description:
 
 * Threshold is for how close the image match should be. If you have a template that is reporting success when it shouldnt be, turn this up.
 * Reduction is the percentage the images are reduced to, with a value of 20, the source and template image are reduced to 20% of their actual size. The smaller the size, the faster it takes to evaluate but at the cost of accuracy.
 * Split takes a string input that must be one of the following: `"All","Top","Bottom","Left","Right","Q1","Q2","Q3","Q4","Custom"` `All` means the source is not split at all. `Top,Bottom,Left,Right` split the source image in half, the selected item is the side the template is checked against. `Q1` is top left, `Q2` top right, `Q3` bottom left, and `Q4` is bottom right. `Custom` lets you define a rectangle to crop by.
 * CustomX is the `x` location to start the custom crop
 * CustomY is the `y` location to start the custom crop
 * CustomWidth is the width of your custom crop
 * CustomHeight is the width of your custom crop.
 
 By setting Custom, and giving parameters you can have your template checked against a much smaller area of the screen.
 
#### Examples
```
var arr = array(null,25,"Q3");#ignore threshold, set reduction and split, ignore everything else
var set = SetTemplateDefaultOptions(arr);
PrintLine(set);
#prints [90,25,Q4,0,0,0,0]
#*the variable assignment is not required*
```



| [Back]() |