namespace Manssiere
{
    using Core.DemoFlow;
    using Core.Graphics.Transition;
    using Effects;

    public class DemoFlow : AbstractDemoFlow
    {
        public DemoFlow()
        {
            FadeIn<StamperScene>()
                .TransitionTo<ScotchYokeUserControl>().Using<CrossFade>()
                .TransitionTo<LoonyGears>().Using<CrossFade>()
                .TransitionTo<Viewport3DTestWindow>().Using<Swipe>();
        }
    }
}