namespace Manssiere.Core.Graphics.Effects
{
    /// <summary>
    /// Blending modes.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// NORMAL is often used as the default blending mode. The blend image is placed over the base
        /// image. The resulting image equals the blend image when the opacity is 1.0 (i.e., the base
        /// image is completely covered). For opacities other than 1.0, the result is a linear blend of the
        /// two images based on Opacity.
        /// </summary>
        Normal,

        /// <summary>
        /// The AVERAGE blend mode adds the two images and divides by two. The result is the same as 
        /// NORMAL when the opacity is set to 0.5. This operation is commutative.
        /// </summary>
        Average,

        /// <summary>
        /// BEHIND chooses the blend value only where the base image is completely transparent (i.e., base.a
        /// = 0.0). You can think of the base image as a piece of clear acetate, and the effect of this mode
        /// is as if you were painting the blend image on the back of the acetateonly the areas painted
        /// behind transparent pixels are visible.
        /// </summary>
        Behind,

        /// <summary>
        /// CLEAR always uses the blend value, and the alpha value of result is set to 0 (transparent). This
        /// blend mode is more apt to be used with drawing tools than on complete images.
        /// </summary>
        Clear,

        /// <summary>
        /// In DARKEN mode, the two values are compared, and the minimum value is chosen for each
        /// component. This operation makes images darker because the blend image can do nothing
        /// except make the base image darker. A blend image that is completely white (RGB = 1.0, 1.0,
        /// 1.0) does not alter the base image. Regions of black (0, 0, 0) in either image cause the result
        /// to be black. It is commutativethe result is the same if the blend image and the base image are
        /// swapped.
        /// </summary>
        Darken,

        /// <summary>
        /// LIGHTEN can be considered the opposite of DARKEN. Instead of taking the minimum of each
        /// component, we take the maximum. The blend image can therefore never do anything but make
        /// the result lighter. A blend image that is completely black (RGB = 0, 0, 0) does not alter the
        /// base image. Regions of white (1.0, 1.0, 1.0) in either image cause the result to be white. The
        /// operation is commutative because swapping the two images does not change the result.
        /// </summary>
        Lighten,

        /// <summary>
        /// In MULTIPLY mode, the two values are multiplied together. This produces a darker result in all
        /// areas in which neither image is completely white. White is effectively an identity (or
        /// transparency) operator because any color multiplied by white will be the original color. Regions
        /// of black (0, 0, 0) in either image cause the result to be black. The result is similar to the effect
        /// of stacking two color transparencies on an overhead projector. This operation is commutative.
        /// </summary>
        Multiply,

        /// <summary>
        /// SCREEN can be thought of as the opposite of MULTIPLY because it multiplies the inverse of the
        /// two input values. The result of this multiplication is then inverted to produce the final result.
        /// Black is effectively an identity (or transparency) operator because any color multiplied by the
        /// inverse of black (i.e., white) will be the original color. This blend mode is commutative.
        /// </summary>
        Screen,

        /// <summary>
        /// COLOR BURN darkens the base color as indicated by the blend color by decreasing luminance.
        /// There is no effect if the blend value is white. This computation can result in some values less
        /// than 0, so truncation may occur when the resulting color is clamped.
        /// </summary>
        ColorBurn,

        /// <summary>
        /// COLOR DODGE brightens the base color as indicated by the blend color by increasing
        /// luminance. There is no effect if the blend value is black. This computation can result in some
        /// values greater than 1, so truncation may occur when the result is clamped.
        /// </summary>
        ColorDodge,

        /// <summary>
        /// SOFT LIGHT produces an effect similar to a soft (diffuse) light shining through the blend image
        /// and onto the base image. The resulting image is essentially a muted combination of the two
        /// images.
        /// </summary>
        SoftLight,

        /// <summary>
        /// HARD LIGHT mode is identical to OVERLAY mode, except that the luminance value is computed
        /// with the blend value rather than the base value. The effect is similar to shining a harsh light
        /// through the blend image and onto the base image. Pixels in the blend image with a luminance
        /// of 0.5 have no effect on the base image. This mode is often used to produce embossing effects.
        /// The mix function provides a linear blend between the two functions for luminance in the range
        /// [0.45,0.55].
        /// </summary>
        HardLight,

        /// <summary>
        /// In the ADD mode, the result is the sum of the blend image and the base image. Truncation may
        /// occur because resulting values can exceed 1.0. The blend and base images can be swapped,
        /// and the result will be the same.
        /// </summary>
        Add,

        /// <summary>
        /// SUBTRACT subtracts the blend image from the base image. Truncation may occur because
        /// resulting values may be less than 0.
        /// </summary>
        Substract,

        /// <summary>
        /// In the DIFFERENCE mode, the result is the absolute value of the difference between the blend
        /// value and the base value. A result of black means the two initial values were equal. A result of
        /// white means they were opposite. This mode can be useful for comparing images because
        /// identical images produce a completely black result. An all-white blend image can be used to
        /// invert the base image. Blending with black produces no change. Because of the absolute value
        /// operation, this blend mode is commutative.
        /// </summary>
        Difference,

        /// <summary>
        /// The INVERSE DIFFERENCE blend mode performs the "opposite" of DIFFERENCE. Blend values of
        /// white and black produce the same results as for DIFFERENCE (white inverts and black has no
        /// effect), but colors in between white and black become lighter instead of darker. This operation
        /// is commutative.
        /// </summary>
        InverseDifference,

        /// <summary>
        /// EXCLUSION is similar to DIFFERENCE, but it produces an effect that is lower in contrast
        /// (softer). The effect for this mode is in between the effects of the DIFFERENCE and INVERSE
        /// DIFFERENCE modes. Blending with white inverts the base image, blending with black has no
        /// effect, and colors in between become gray. This is also a commutative blend mode.
        /// </summary>
        Exclusion
    }
}