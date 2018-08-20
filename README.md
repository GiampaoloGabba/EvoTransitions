# EvoTransitions
Custom NavigationRenderers for Xamarin.Forms (IOS and Android) to activate shared element transitions between two screens.

Its a <b>proof of concept</b> with the following limitations:

1) Works only between pages in a NavigationPage container
2) Transitions betweeen listview and details page are not currently supported (coming soon)
3) To save time, my custom renderers export directly to NavigationPage.
In your projects its better to create a custom NavigationPage and use it only when transitions are needed.
 
## Basic usage

- Use the TagEffect to attach numeric tags to the views you want to animate (in both source and destination page)
- Source and destination views need to have the same numeric tag
- Every tag in a single page needs to be unique
- In IOS, after a push, you can use the swipe to right (from the left edge) to get back to the previous page. The transition will follow the span gesture

## Examples

**Android** *(Poor quality gif with dogs!)*<br><br>
<img src="http://www.evolutionlab.it/github/transition_droid.gif" height="500">
<br><br>

**IOS** *(FLuid video with cats!)*<br><br>
<a href="https://www.youtube.com/watch?v=A826mg30D7" target="_blank"><img src="https://img.youtube.com/vi/A826mg30D74/0.jpg"></a>

## References

- Xamarin.Forms issue: <a href="https://github.com/xamarin/Xamarin.Forms/issues/3334">#3334</a>
- Repository that pointed me in the right direction: <a href="https://github.com/GalaxiaGuy/SharedElementTest">GalaxiaGuy/SharedElementTest</a>


