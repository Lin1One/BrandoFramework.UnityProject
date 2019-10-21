using UnityEngine;
using System.Collections;
using System;

// A UV Transform is a transform considering only scale and offset it is used to represent the scaling and offset
// from UVs outside the 0,0..1,1 box and material tiling
// Rect objects are used to store the transform

namespace GameWorld
{
    public class UVRectUtility
    {
        public static void Test()
        {
            /*
            Debug.Log("Running test");
            DRect rawUV = new DRect(.25, .25, 1.5, 1.5);
            DRect rawMat = new DRect(.5, .5, 2, 2);
            DRect rawCombined = CombineTransforms(ref rawUV, ref rawMat);
            DRect matInvHierarchy = new DRect(rawMat.GetRect());
            InvertHierarchy(ref rawUV, ref matInvHierarchy);
            DRect invertHierarchyCombined = CombineTransforms(ref matInvHierarchy, ref rawUV);

            //These transforms should be the same
            Debug.Log("These should be same " + rawCombined + " " + invertHierarchyCombined);

            //New transform that should fit in  combined
            DRect otherUV = new DRect(1,1,1.5,1.5);
            DRect otherMat = new DRect(0, 0, 1, 1);
            DRect otherCombined = CombineTransforms(ref otherUV, ref otherMat);
            Debug.Log("Other : " + otherCombined);
            Debug.Log("Other fits = " + RectContains(ref rawCombined, ref otherCombined));

            DRect invOtherCombined = InverseTransform(ref otherCombined);
            Debug.Log(TransformPoint(ref otherCombined, new Vector2(0, 0)) + " " + TransformPoint(ref otherCombined, new Vector2(1, 1)));
            Debug.Log(TransformPoint(ref invOtherCombined, new Vector2(0,0)) + " " + TransformPoint(ref invOtherCombined, new Vector2(1,1)).ToString("f5")
                                         + " " + TransformPoint(ref invOtherCombined, new Vector2(2, 2)).ToString("f5")
                                          + " " + TransformPoint(ref invOtherCombined, new Vector2(3, 3)).ToString("f5"));

            DRect src2combined = CombineTransforms(ref invOtherCombined, ref rawCombined);

            Debug.Log(TransformPoint(ref src2combined, new Vector2(0, 0)) + " " + TransformPoint(ref src2combined, new Vector2(1, 1)).ToString("f5")
                                         + " " + TransformPoint(ref src2combined, new Vector2(2, 2)).ToString("f5")
                                          + " " + TransformPoint(ref src2combined, new Vector2(3, 3)).ToString("f5"));
            */
            //DRect rawUV = new DRect(0, 0, 1, 1);
            DRect rawMat = new DRect(.5, .5, 2, 2);
            DRect fullSample = new DRect(.25,.25,3,3);
            //DRect altasRect = new DRect(0, 0, 1, 1);

            DRect invRawMat = InverseTransform(ref rawMat);
            DRect invFullSample = InverseTransform(ref fullSample);
            DRect relativeTransform = CombineTransforms(ref rawMat, ref invFullSample);
            Debug.Log(invRawMat);
            Debug.Log(relativeTransform);
            Debug.Log("one mat trans " + TransformPoint(ref rawMat, new Vector2(1, 1)));
            Debug.Log("one inv mat trans " + TransformPoint(ref invRawMat, new Vector2(1, 1)).ToString("f4"));
            Debug.Log("zero " + TransformPoint(ref relativeTransform, new Vector2(0, 0)).ToString("f4"));
            Debug.Log("one " + TransformPoint(ref relativeTransform, new Vector2(1, 1)).ToString("f4"));
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="tilingTreatment"></param>
        /// <param name="uvR"></param>
        /// <param name="sourceMaterialTiling"></param>
        /// <param name="samplingEncapsulatinRect"></param>
        /// <returns></returns>
        public static bool IsMeshAndMaterialRectEnclosedByAtlasRect(TextureTilingTreatment tilingTreatment,
            Rect uvR,
            Rect sourceMaterialTiling,
            Rect samplingEncapsulatinRect)

        {
            Rect potentialRect = new Rect();
            potentialRect = CombineTransforms(ref uvR, ref sourceMaterialTiling);
            Debug.Log("IsMeshAndMaterialRectEnclosedByAtlasRect Rect in atlas uvR=" + uvR.ToString("f5") +
                " sourceMaterialTiling=" + sourceMaterialTiling.ToString("f5") +
                "Potential Rect (must fit in encapsulating) " + potentialRect.ToString("f5") +
                " encapsulating=" + samplingEncapsulatinRect.ToString("f5") +
                " tilingTreatment=" + tilingTreatment);

            if (tilingTreatment == TextureTilingTreatment.edgeToEdgeX)
            {
                if (LineSegmentContainsShifted(samplingEncapsulatinRect.y, samplingEncapsulatinRect.height, potentialRect.y, potentialRect.height))
                {
                    return true;
                }
            }
            else if (tilingTreatment == TextureTilingTreatment.edgeToEdgeY)
            {
                if (LineSegmentContainsShifted(samplingEncapsulatinRect.x, samplingEncapsulatinRect.width, potentialRect.x, potentialRect.width))
                {
                    return true;
                }
            }
            else if (tilingTreatment == TextureTilingTreatment.edgeToEdgeXY)
            {
                //only one rect in atlas and is edge to edge in both X and Y directions.
                return true;
            }
            else
            {
                if (RectContainsShifted(ref samplingEncapsulatinRect, ref potentialRect))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  x 轴变换
        /// </summary>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float TransformX(DRect r, double x)
        {
            return (float) (r.width * x + r.x);
        }

        /// <summary>
        /// 矩阵变换
        /// r1 为原始 Rect
        /// r2 为归一化矩阵
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static DRect CombineTransforms(ref DRect r1, ref DRect r2)
        {
            DRect rCombined = new DRect(
                r1.x * r2.width + r2.x,
                r1.y * r2.height + r2.y,
                r1.width * r2.width,
                r1.height * r2.height);
            return rCombined;
        }

        public static Rect CombineTransforms(ref Rect r1, ref Rect r2)
        {
            Rect rCombined = new Rect(
                r1.x * r2.width + r2.x,
                r1.y * r2.height + r2.y,
                r1.width * r2.width,
                r1.height * r2.height);
            return rCombined;
        }

        /// <summary>
        /// 取逆矩形
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DRect InverseTransform(ref DRect t)
        {
            DRect tinv = new DRect();
            tinv.x = -t.x / t.width;
            tinv.y = -t.y / t.height;
            tinv.width = 1f / t.width;
            tinv.height = 1f / t.height;
            return tinv;
        }

        public static DRect GetShiftTransformToFitBinA(ref DRect A, ref DRect B)
        {
            DVector2 ac = A.center;
            DVector2 bc = B.center;
            DVector2 diff = DVector2.Subtract(ac, bc);
            double dx = Convert.ToInt32(diff.x);
            double dy = Convert.ToInt32(diff.y);
            return new DRect(dx,dy,1.0,1.0);
        }

        /// <summary>
        /// shifts willBeIn so it is centered in uvRect1, then find a rect that encloses both
        /// </summary>
        /// <param name="uvRect1"></param>
        /// <param name="willBeIn"></param>
        /// <returns></returns>
        public static DRect GetEncapsulatingRectShifted(ref DRect uvRect1, ref DRect willBeIn)
        {
            DVector2 bc = uvRect1.center;
            DVector2 tfc = willBeIn.center;
            DVector2 diff = DVector2.Subtract(bc, tfc);
            double dx = Convert.ToInt32(diff.x);
            double dy = Convert.ToInt32(diff.y);
            DRect uvRect2 = new DRect(willBeIn);
            uvRect2.x += dx;
            uvRect2.y += dy;
            double smnx = uvRect1.x;
            double smny = uvRect1.y;
            double smxx = uvRect1.x + uvRect1.width;
            double smxy = uvRect1.y + uvRect1.height;
            double bmnx = uvRect2.x;
            double bmny = uvRect2.y;
            double bmxx = uvRect2.x + uvRect2.width;
            double bmxy = uvRect2.y + uvRect2.height;
            double minx, miny, maxx, maxy;
            minx = maxx = smnx;
            miny = maxy = smny;
            if (bmnx < minx) minx = bmnx;
            if (smnx < minx) minx = smnx;
            if (bmny < miny) miny = bmny;
            if (smny < miny) miny = smny;
            if (bmxx > maxx) maxx = bmxx;
            if (smxx > maxx) maxx = smxx;
            if (bmxy > maxy) maxy = bmxy;
            if (smxy > maxy) maxy = smxy;
            DRect uvRectCombined = new DRect(minx, miny, maxx - minx, maxy - miny);
            return uvRectCombined;
        }

        public static DRect GetEncapsulatingRect(ref DRect uvRect1, ref DRect uvRect2)
        {
            double smnx = uvRect1.x;
            double smny = uvRect1.y;
            double smxx = uvRect1.x + uvRect1.width;
            double smxy = uvRect1.y + uvRect1.height;
            double bmnx = uvRect2.x;
            double bmny = uvRect2.y;
            double bmxx = uvRect2.x + uvRect2.width;
            double bmxy = uvRect2.y + uvRect2.height;
            double minx, miny, maxx, maxy;
            minx = maxx = smnx;
            miny = maxy = smny;
            if (bmnx < minx) minx = bmnx;
            if (smnx < minx) minx = smnx;
            if (bmny < miny) miny = bmny;
            if (smny < miny) miny = smny;
            if (bmxx > maxx) maxx = bmxx;
            if (smxx > maxx) maxx = smxx;
            if (bmxy > maxy) maxy = bmxy;
            if (smxy > maxy) maxy = smxy;
            DRect uvRectCombined = new DRect(minx, miny, maxx - minx, maxy - miny);
            return uvRectCombined;
        }

        public static bool RectContainsShifted(ref DRect bucket, ref DRect tryFit)
        {
            //get the centers of bucket and tryFit
            DVector2 bc = bucket.center;
            DVector2 tfc = tryFit.center;
            DVector2 diff = DVector2.Subtract(bc, tfc);
            double dx = Convert.ToInt32(diff.x);
            double dy = Convert.ToInt32(diff.y);
            DRect tmp = new DRect(tryFit);
            tmp.x += dx;
            tmp.y += dy;
            return bucket.Encloses(tmp);
        }

        public static bool RectContainsShifted(ref Rect bucket, ref Rect tryFit)
        {
            //get the centers of bucket and tryFit
            Vector2 bc = bucket.center;
            Vector2 tfc = tryFit.center;
            Vector2 diff = bc - tfc;
            float dx = Convert.ToInt32(diff.x);
            float dy = Convert.ToInt32(diff.y);
            Rect tmp = new Rect(tryFit);
            tmp.x += dx;
            tmp.y += dy;
            return RectContains(ref bucket, ref tmp);
        }

        public static bool LineSegmentContainsShifted(float bucketOffset, float bucketLength, float tryFitOffset, float tryFitLength)
        {
            Debug.Assert(bucketLength >= 0);
            Debug.Assert(tryFitLength >= 0);
            float bc = bucketOffset + bucketLength / 2f;
            float tfc = tryFitOffset + tryFitLength / 2f;
            float diff = bc - tfc;
            float delta = Convert.ToInt32(diff);
            tryFitOffset += delta;
            float sminx = tryFitOffset;
            float smaxx = tryFitOffset + tryFitLength;
            float bminx = bucketOffset - 10e-3f;
            float bmaxx = bucketOffset + bucketLength + 10e-3f;
            return bminx <= sminx && sminx <= bmaxx &&
                   bminx <= smaxx && smaxx <= bmaxx;
        }

        public static bool RectContains(ref DRect bigRect, ref DRect smallToTestIfFits)
        {
            double sminx = smallToTestIfFits.x;
            double sminy = smallToTestIfFits.y;
            double smaxx = smallToTestIfFits.x + smallToTestIfFits.width;
            double smaxy = smallToTestIfFits.y + smallToTestIfFits.height;
            //expand slightly to deal with rounding errors
            double bminx = bigRect.x - 10e-3f;
            double bminy = bigRect.y - 10e-3f;
            double bmaxx = bigRect.x + bigRect.width + 10e-3f;
            double bmaxy = bigRect.y + bigRect.height + 10e-3f;

            return bminx <= sminx && sminx <= bmaxx &&
                    bminx <= smaxx && smaxx <= bmaxx &&
                    bminy <= sminy && sminy <= bmaxy &&
                    bminy <= smaxy && smaxy <= bmaxy;
        }

        public static bool RectContains(ref Rect bigRect, ref Rect smallToTestIfFits)
        {
            float smnx = smallToTestIfFits.x;
            float smny = smallToTestIfFits.y;
            float smxx = smallToTestIfFits.x + smallToTestIfFits.width;
            float smxy = smallToTestIfFits.y + smallToTestIfFits.height;
            //expand slightly to deal with rounding errors
            float bmnx = bigRect.x - 10e-3f;
            float bmny = bigRect.y - 10e-3f;
            float bmxx = bigRect.x + bigRect.width + 10e-3f;
            float bmxy = bigRect.y + bigRect.height + 10e-3f;

            return bmnx <= smnx && smnx <= bmxx &&
                    bmnx <= smxx && smxx <= bmxx &&
                    bmny <= smny && smny <= bmxy &&
                    bmny <= smxy && smxy <= bmxy;
        }

        public static Vector2 TransformPoint(ref DRect r, Vector2 p)
        {
            return new Vector2((float) (r.width * p.x + r.x),(float)(r.height * p.y + r.y));
        }

        public static DVector2 TransformPoint(ref DRect r, DVector2 p)
        {
            return new DVector2((r.width * p.x + r.x), (r.height * p.y + r.y));
        }
    }
}
