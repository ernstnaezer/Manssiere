Manssiere
=========
Manssiere is a play project for rendering WPF/XAML elements in OpenGL.
It was written to participate in a demo coding contest at Breakpoint 2010 but never got finished. 

I'm releasing the rendering source code for somebody to find it fun and amusing.

Sample stuff
------------
You can use Micosoft Expression Blend to create new windows or user control that contain the stuff to be rendered. After loading these files into Manssiere you can use the the DemoFlow class to define which part should be exected and how to go to the next.

    public class DemoFlow 
        : AbstractDemoFlow
    {
        public DemoFlow()
        {
            FadeIn<StamperScene>()
                .TransitionTo<ScotchYokeUserControl>().Using<CrossFade>()
                .TransitionTo<LoonyGears>().Using<CrossFade>()
                .TransitionTo<Viewport3DTestWindow>().Using<Swipe>();        }
    }

The fun thing is that you can do code behind as well. Take a look at the piston motion control.

At runtime you can press the following keys:

* Escape 			-> exits the application
* P					-> take a screenshot (saved on the desktop)
* Space / Right		-> Next effect
* Backspace / Left	-> Previous effect

Inner workings
--------------
The following XAML elements can be rendered in opengl:

* Canvas Rectangles
* Images
* Free form paths
* Viewport 3D elements including lights

The Microsoft XAML engine is abused to do the heavy lifting. They have defined a really nice hierarchical system for defining graphical elements in 2d space and applying transformations to those object over time using keyframers. The rendering engine simulates a timer that triggers WPF to update the internal state of the elements, after this is done we simply itterate over the result and tesselate this to an opengl canvas.

There is a lot more stuff inside but it's all more or less testing code. So just dive in and take a look.

Screenshots
-----------
![screenshot](http://blog.ernstnaezer.nl/image.axd?picture=2011%2f9%2fscreenshot+-+2011-09-03+17.53.35.png)
![screenshot](http://blog.ernstnaezer.nl/image.axd?picture=2011%2f9%2fscreenshot+-+2011-09-03+18.11.22.png)
![screenshot](http://blog.ernstnaezer.nl/image.axd?picture=2011%2f9%2fscreenshot+-+2011-09-03+18.11.23.png)
![screenshot](http://blog.ernstnaezer.nl/image.axd?picture=2011%2f9%2fscreenshot+-+2011-09-03+18.31.21.png)
![screenshot](http://blog.ernstnaezer.nl/image.axd?picture=2011%2f9%2fscreenshot+-+2011-09-03+18.31.20.png)
![screenshot](http://blog.ernstnaezer.nl/image.axd?picture=2011%2f9%2fscreenshot+-+2011-09-03+18.35.01.png)

License
-------
Copyright 2011 Ernst Naezer
 
Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
this file except in compliance with the License. You may obtain a copy of the 
License at 

    http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software distributed 
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied. See the License for the 
specific language governing permissions and limitations under the License.
