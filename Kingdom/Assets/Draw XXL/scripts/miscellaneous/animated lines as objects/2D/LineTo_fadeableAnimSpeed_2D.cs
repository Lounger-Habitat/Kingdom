﻿namespace DrawXXL
{
    using UnityEngine;

    public class LineTo_fadeableAnimSpeed_2D : ParentOf_Lines_fadeableAnimSpeed_2D
    {
        public Vector2 direction = Vector2.one;
        public Vector2 end = Vector2.zero;
        public Color endColor = default(Color); //Can be ignored if the line should have only one color. If it is specified then the line will have a fading color transition, starting with "color"(link) at the start of the line and ending with "endColor" at the end of the line
        public DrawBasics.LineStyle style = DrawBasics.LineStyle.solid;
        public float stylePatternScaleFactor = 1.0f;
        public float alphaFadeOutLength_0to1 = 0.0f;
        public bool skipPatternEnlargementForLongLines = false;
        public bool skipPatternEnlargementForShortLines = false;

        public LineTo_fadeableAnimSpeed_2D(Vector2 direction, Vector2 end)
        {
            this.direction = direction;
            this.end = end;
        }

        public void Draw()
        {
            if (DXXLWrapperForUntiysBuildInDrawLines.CheckIfDrawingIsCurrentlySkipped()) { return; }
            if (UtilitiesDXXL_Colors.IsDefaultColor(endColor))
            {
                lineAnimationProgress = InternalDraw(direction, end, color, width, text, style, custom_zPos, stylePatternScaleFactor, animationSpeed, lineAnimationProgress, endPlates_size, alphaFadeOutLength_0to1, enlargeSmallTextToThisMinTextSize, durationInSec, hiddenByNearerObjects, skipPatternEnlargementForLongLines, skipPatternEnlargementForShortLines);
            }
            else
            {
                lineAnimationProgress = InternalDraw_withColorFade(direction, end, color, endColor, width, text, style, custom_zPos, stylePatternScaleFactor, animationSpeed, lineAnimationProgress, endPlates_size, alphaFadeOutLength_0to1, enlargeSmallTextToThisMinTextSize, durationInSec, hiddenByNearerObjects, skipPatternEnlargementForLongLines, skipPatternEnlargementForShortLines);
            }
        }

        public static LineAnimationProgress InternalDraw(Vector2 direction, Vector2 end, Color color, float width, string text, DrawBasics.LineStyle style, float custom_zPos, float stylePatternScaleFactor, float animationSpeed, LineAnimationProgress precedingLineAnimationProgress, float endPlates_size, float alphaFadeOutLength_0to1, float enlargeSmallTextToThisMinTextSize, float durationInSec, bool hiddenByNearerObjects, bool skipPatternEnlargementForLongLines, bool skipPatternEnlargementForShortLines)
        {
            if (DXXLWrapperForUntiysBuildInDrawLines.CheckIfDrawingIsCurrentlySkipped()) { return null; }
            if (UtilitiesDXXL_Log.ErrorLogForInvalidVectors(end, "end")) { return null; }
            if (UtilitiesDXXL_Log.ErrorLogForInvalidVectors(direction, "direction")) { return null; }
            return Ray_fadeableAnimSpeed_2D.InternalDraw(end - direction, direction, color, width, text, style, custom_zPos, stylePatternScaleFactor, animationSpeed, precedingLineAnimationProgress, endPlates_size, alphaFadeOutLength_0to1, enlargeSmallTextToThisMinTextSize, durationInSec, hiddenByNearerObjects, skipPatternEnlargementForLongLines, skipPatternEnlargementForShortLines);
        }

        public static LineAnimationProgress InternalDraw_withColorFade(Vector2 direction, Vector2 end, Color startColor, Color endColor, float width, string text, DrawBasics.LineStyle style, float custom_zPos, float stylePatternScaleFactor, float animationSpeed, LineAnimationProgress precedingLineAnimationProgress, float endPlates_size, float alphaFadeOutLength_0to1, float enlargeSmallTextToThisMinTextSize, float durationInSec, bool hiddenByNearerObjects, bool skipPatternEnlargementForLongLines, bool skipPatternEnlargementForShortLines)
        {
            if (DXXLWrapperForUntiysBuildInDrawLines.CheckIfDrawingIsCurrentlySkipped()) { return null; }
            if (UtilitiesDXXL_Log.ErrorLogForInvalidVectors(end, "end")) { return null; }
            if (UtilitiesDXXL_Log.ErrorLogForInvalidVectors(direction, "direction")) { return null; }
            return Ray_fadeableAnimSpeed_2D.InternalDrawColorFade(end - direction, direction, startColor, endColor, width, text, style, custom_zPos, stylePatternScaleFactor, animationSpeed, precedingLineAnimationProgress, endPlates_size, alphaFadeOutLength_0to1, enlargeSmallTextToThisMinTextSize, durationInSec, hiddenByNearerObjects, skipPatternEnlargementForLongLines, skipPatternEnlargementForShortLines);
        }

    }

}
