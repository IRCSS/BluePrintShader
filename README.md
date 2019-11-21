Blueprint rendering with depth peeling in Unity
=================

This repo contains a setup to render blueprints of objects. A blueprint shows visible and non visible edges of an object. The basic of the techinque is a combination of edge enhancement and depth peeling. For each layer of depth an edge map is constructed which combined creates the blue print rendering, for more: https://medium.com/@shahriyarshahrabi/blueprint-shader-with-depth-peeling-in-unity-a8eda80da8c3

![screenshot](https://media.giphy.com/media/XE7OrTA84wxIsBKyNo/giphy.gif)

Known Issues
=================
There is support for only rendering a single mesh. This can easily be extended for other meshes. 