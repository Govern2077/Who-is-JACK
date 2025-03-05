# Colored Pencils - Unity3D Image Effect
#### (c) 2018 by [flockaroo](http://www.flockaroo.at) (Florian Berger) - email: <flockaroo@gmail.com>

******

### How to use

Select your camera node and then simply add "ColoredPencilsEffect" script to camera components (can be found in Assets/flockaroo/ColoredPencils/).
You can drag/drop it to there or choose it from the menu (Component/Scripts/Flockaroo/ColoredPencils).

![How to use - Image](/unityprj/Unity_ColoredPencils/Assets/flockaroo/ColoredPencils/howto.png){ width="100%" }

__Warning!!__ The subfolder "flockaroo_[effect name]" in "Resources" is needed by the effect script for unique identification of files and should not be removed or renamed.

<div style="page-break-after: always;"></div>

### Parameters

The shader provides the following parameters:

#### Input/Output
 | Parameter       | function
 |-----------------|--------------
 | Input Texture   | take this texture as input instead of the camera
 | Render To Texture | render to texture instead of screen
 | Output Texture  | texture being rendered to if above is checked
 | Output Mipmap   | generate mipmap for output texture

#### Main faders
 | Parameter       | function
 |-----------------|--------------
 | Fade            | 0 = effect image ... 1 = original content
 | Pan Fade        | 0 = effect image ... 1 = original content - pan from left to right

#### Source
 | Parameter       | function
 |-----------------|--------------
 | Brightness      | adjust brightness of the content before applying the effect
 | Contrast        | adjust contrast of the content before applying the effect
 | Color           | the color intensity of the effect

#### Effect
 | Parameter       | function
 |-----------------|--------------
 | Shader Method   | 0 = original shader <br> 1 = newer version (faster, other color scheme) <br> 2 = even faster
 | Outlines        | strength of the pencil outlines
 | Outline Color   | color of outlines
 | Hatches         | strength of the pencil hatches
 | Outline Error   | drawing-error of the pencil outlines
 | Flicker         | activates a flicker effect on cross-hatches and outlines
 | Flicker Freq    | flicker frequency in Hz
 | Fixed Hatch Dir | makes crosshatches all parallel and content independent
 | Precalc Gradient | uses a precalucated gradient (only in Shader Method 2!!)
 | Precalc Gradient Flip | to y-flips precalculated gradient if necessary
 | Hatch Scale     | scale of the hatch strokes (line thickness)
 | Hatch Angle     | rotate hatches
 | Hatch Length    | length of the hatch strokes
 | Mip Level       | affects the detail of the strokes, and slightly disorients the strokes direction close to color jumps (only works properly for Unity versions higher than 5.5)
 | Vignetting      | darkening the window border
 | Content Vignetting | fade content to white paper on border

<div style="page-break-after: always;"></div>

#### Background
 | Parameter       | function
 |-----------------|--------------
 | Paper Tint      | color of the paper being drawn on
 | Paper Roughness | roughness of paper surface
 | Paper Texture   | custom paper texture (background)

#### Other
 | Parameter       | function
 |-----------------|--------------
 | Flip Y          | image Y flip
 | HDRP Gamma      | check this if you are using linear color space (only active in hdrp mode)

### HDRP (disabled by default)
The hdrp file is disabled by default !!! here's how to use it: <br>
Unity wont compile this effect properly if no hdrp support is present
on your version, so in the hdrp ".cs" file in the very first line the "//#USE_HDRP" must be uncommmented to make use the hdrp effect.<br>
You also have to add it to the list of effects known to your project:<br>
"Edit/Project Settings... -> HDRP Default Settings -> After Post Process"<br>
..and then add it as an effect volume by clicking "Add Override" and the
selecting <br>"Post-processing/Custom/Flockaroo/..." <br>from the menu.


### URP (disabled by default)
The URP file is disabled by default !!! here's how to use it: <br>
Unity wont compile this effect properly if no URP-support is present
on your version, so in the urp "...URP.cs" file in the very first line the "//#USE_URP" must be uncommmented to make use the urp effect.<br>
Then under "Assets/Settings/ForwardRenderer" press "Add Renderer Feature" in the Inspector Tab.
<br>
<br>
![How to use URP - Image](howto_urp.png){ width="100%" }
<br>
<br>
BEWARE!! For now the effect can not be used after Post Processing. <br>Furthermore some Post-Processing-Effects like "Bloom" dont work properly. Disable those effects for proper functionality.
