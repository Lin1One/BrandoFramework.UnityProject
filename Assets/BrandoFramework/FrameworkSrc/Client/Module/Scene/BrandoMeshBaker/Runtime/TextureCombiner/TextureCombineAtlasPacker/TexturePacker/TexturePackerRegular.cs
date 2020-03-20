using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    public class TexturePackerRegular : TexturePacker
    {		
		class ProbeResult
        {
			public int w;
			public int h;
            public int outW;
            public int outH;
			public AtalsAreaNode root;
			public bool largerOrEqualToMaxDim;
			public float efficiency;
			public float squareness;

            //these are for the multiAtlasPacker
            public float totalAtlasArea;
            public int numAtlases;
			
			public void Set(int ww, int hh, int outw, int outh, AtalsAreaNode r, bool fits, float e, float sq)
            {
				w = ww;
				h = hh;
                outW = outw;
                outH = outh;
				root = r;
				largerOrEqualToMaxDim = fits;
				efficiency = e;
				squareness = sq;
			}
			
			public float GetScore(bool doPowerOfTwoScore)
            {
				float fitsScore = largerOrEqualToMaxDim ? 1f : 0f;
				if (doPowerOfTwoScore){
					return fitsScore * 2f + efficiency;
				} else {
					return squareness + 2 * efficiency + fitsScore;
				}
			}

            public void PrintTree()
            {
                printTree(root, "  ");
            }

            static void printTree(AtalsAreaNode r, string spc)
            {
                Debug.Log(spc + "Nd img=" + (r.img != null) + " r=" + r.nodeAreaRect);
                if (r.child[0] != null)
                    printTree(r.child[0], spc + "      ");
                if (r.child[1] != null)
                    printTree(r.child[1], spc + "      ");
            }
        }
		
		internal class AtalsAreaNode
        {
            internal NodeType isFullAtlas; //is this node a full atlas used for scaling to fit  
            internal AtalsAreaNode[] child = new AtalsAreaNode[2];
            internal AtalsAreaPixRect nodeAreaRect;
            internal ImageAreaInAtlas img;
            ProbeResult bestRoot;
            internal AtalsAreaNode(NodeType rootType)
            {
                isFullAtlas = rootType;
            }

            private bool isLeaf()
            {
				if (child[0] == null || child[1] == null)
                {
					return true;
				}
				return false;
			}
			
			internal AtalsAreaNode Insert(ImageAreaInAtlas im, bool handed)
            {
				int a,b;
				if (handed)
                {
				  a = 0;
				  b = 1;
				}
                else
                {
				  a = 1;
				  b = 0;
				}
				if (!isLeaf())
                {   //不是叶
					//try insert into first child
					AtalsAreaNode newNode = child[a].Insert(im,handed);
					if (newNode != null)
						return newNode;
					//no room insert into second
					return child[b].Insert(im,handed);
				}
                else
                {
			        //(if there's already a img here, return)
			        if (img != null) 
						return null;
			
			        //(if space too small, return)
			        if (nodeAreaRect.w < im.w || nodeAreaRect.h < im.h)
			            return null;

                    //(if space just right, accept)
                    if (nodeAreaRect.w == im.w && nodeAreaRect.h == im.h)
                    {
                        img = im;
                        return this;
                    }

                    //拆分此节点并创建两个分支
                    child[a] = new AtalsAreaNode(NodeType.regular);
			        child[b] = new AtalsAreaNode(NodeType.regular);
			        
			        //(decide which way to split)
			        int dw = nodeAreaRect.w - im.w;
			        int dh = nodeAreaRect.h - im.h;
			        
			        if (dw > dh)
                    {
			            child[a].nodeAreaRect = new AtalsAreaPixRect(nodeAreaRect.x, nodeAreaRect.y, im.w, nodeAreaRect.h);
			            child[b].nodeAreaRect = new AtalsAreaPixRect(nodeAreaRect.x + im.w, nodeAreaRect.y, nodeAreaRect.w - im.w, nodeAreaRect.h);
					}
                    else
                    {
			            child[a].nodeAreaRect = new AtalsAreaPixRect(nodeAreaRect.x, nodeAreaRect.y, nodeAreaRect.w, im.h);
			            child[b].nodeAreaRect = new AtalsAreaPixRect(nodeAreaRect.x, nodeAreaRect.y+ im.h, nodeAreaRect.w, nodeAreaRect.h - im.h);
					}
			        return child[a].Insert(im,handed);				
				}
			}
		}

        ProbeResult bestRoot;

        ////public int atlasY;

        /// <summary>
        /// 展开图集区域树，将图片区域保存在 putHere 列表 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="putHere"></param>
		static void flattenTree(AtalsAreaNode r, List<ImageAreaInAtlas> putHere)
        {
			if (r.img != null)
            {
				r.img.x = r.nodeAreaRect.x;
				r.img.y = r.nodeAreaRect.y;				
				putHere.Add(r.img);
			}
			if (r.child[0] != null)
				flattenTree(r.child[0], putHere);
			if (r.child[1] != null)
				flattenTree(r.child[1], putHere);		
		}
		
		static Texture2D createFilledTex(Color c, int w, int h)
        {
			Texture2D t = new Texture2D(w,h);
			for (int i = 0; i < w; i++)
            {
				for (int j = 0; j < h; j++)
                {
					t.SetPixel(i,j,c);
				}
			}
			t.Apply();
			return t;
		}

        public override AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, int maxDimensionX, int maxDimensionY, int atPadding)
        {
            List<AtlasPadding> padding = new List<AtlasPadding>();
            for (int i = 0; i < imgWidthHeights.Count; i++)
            {
                AtlasPadding p = new AtlasPadding();
                p.leftRight = p.topBottom = atPadding;
                padding.Add(p);
            }
            return GetRects(imgWidthHeights, padding, maxDimensionX, maxDimensionY, false);
        }

        public override AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, List<AtlasPadding> paddings, int maxDimensionX, int maxDimensionY, bool doMultiAtlas)
        {
            Debug.Assert(imgWidthHeights.Count == paddings.Count, imgWidthHeights.Count + " " + paddings.Count);
            int maxPaddingX = 0;
            int maxPaddingY = 0;
            for (int i = 0; i < paddings.Count; i++)
            {
                maxPaddingX = Mathf.Max(maxPaddingX, paddings[i].leftRight);
                maxPaddingY = Mathf.Max(maxPaddingY, paddings[i].topBottom);
            }
            if (doMultiAtlas)
            {
                return _GetRectsMultiAtlas(imgWidthHeights, paddings, maxDimensionX, maxDimensionY, 2 + maxPaddingX * 2, 2 + maxPaddingY * 2, 2 + maxPaddingX * 2, 2 + maxPaddingY * 2);
            }
            else
            {
                AtlasPackingResult apr = _GetRectsSingleAtlas(imgWidthHeights, paddings, maxDimensionX, maxDimensionY, 2 + maxPaddingX * 2, 2 + maxPaddingY * 2, 2 + maxPaddingX * 2, 2 + maxPaddingY * 2, 0);
                if (apr == null)
                {
                    return null;
                }
                else
                {
                    return new AtlasPackingResult[] { apr };
                }
            }
		}

        #region 打包至单个图集

        //------------------ Algorithm for fitting everything into one atlas and scaling down
        // 
        // for images being added calc area, maxW, maxH. A perfectly packed atlas will match area exactly. atlas must be at least maxH and maxW in size.
        // Sort images from big to small using either height, width or area comparer
        // Explore space to find a resonably efficient packing. Grow the atlas gradually until a fit is found
        // Scale atlas to fit
        //
        AtlasPackingResult _GetRectsSingleAtlas(List<Vector2> imgWidthHeights,
            List<AtlasPadding> paddings,
            int maxDimensionX,
            int maxDimensionY,
            int minImageSizeX,
            int minImageSizeY,
            int masterImageSizeX,
            int masterImageSizeY,
            int recursionDepth)
        {
            Debug.Log(string.Format("_GetRects 图片数量 ={0}, 最大尺寸X ={1}, 最小尺寸 X={2}, 最小尺寸 Y ={3}, masterImageSizeX={4}, masterImageSizeY={5}, 递归深度 = {6}",
                imgWidthHeights.Count, maxDimensionX, minImageSizeX, minImageSizeY, masterImageSizeX, masterImageSizeY, recursionDepth));
            if (recursionDepth > 10)
            {
                //最大递归深度设定为 10 
                Debug.LogError("Maximum recursion depth reached. Couldn't find packing for these textures.");
                return null;
            }
            float allImageTotalArea = 0;
            int maxW = 0;
            int maxH = 0;
            ImageAreaInAtlas[] imgsToAdd = new ImageAreaInAtlas[imgWidthHeights.Count];
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                int iw = (int)imgWidthHeights[i].x;
                int ih = (int)imgWidthHeights[i].y;

                ImageAreaInAtlas im = imgsToAdd[i] = new ImageAreaInAtlas(i, iw, ih, paddings[i], minImageSizeX, minImageSizeY);
                allImageTotalArea += im.w * im.h;
                maxW = Mathf.Max(maxW, im.w);
                maxH = Mathf.Max(maxH, im.h);
            }

            if ((float)maxH / (float)maxW > 2)
            {
                Array.Sort(imgsToAdd, new ImageHeightComparer());
            }
            else if ((float)maxH / (float)maxW < .5)
            {
                Array.Sort(imgsToAdd, new ImageWidthComparer());
            }
            else
            {
                Array.Sort(imgsToAdd, new ImageAreaComparer());
            }

            //explore the space to find a resonably efficient packing 
            //探索图片空间以找到合理有效的包装
            int sqrtArea = (int)Mathf.Sqrt(allImageTotalArea);
            int idealAtlasW;
            int idealAtlasH;

            if (atlasMustBePowerOfTwo)
            {
                idealAtlasW = idealAtlasH = RoundToNearestPositivePowerOfTwo(sqrtArea);
                if (maxW > idealAtlasW)
                {
                    idealAtlasW = CeilToNearestPowerOfTwo(idealAtlasW);
                }
                if (maxH > idealAtlasH)
                {
                    idealAtlasH = CeilToNearestPowerOfTwo(idealAtlasH);
                }
            }
            else
            {
                idealAtlasW = sqrtArea;
                idealAtlasH = sqrtArea;
                if (maxW > sqrtArea)
                {
                    idealAtlasW = maxW;
                    idealAtlasH = Mathf.Max(Mathf.CeilToInt(allImageTotalArea / maxW), maxH);
                }
                if (maxH > sqrtArea)
                {
                    idealAtlasW = Mathf.Max(Mathf.CeilToInt(allImageTotalArea / maxH), maxW);
                    idealAtlasH = maxH;
                }
            }

            if (idealAtlasW == 0)
                idealAtlasW = 4;
            if (idealAtlasH == 0)
                idealAtlasH = 4;
            int stepW = (int)(idealAtlasW * .15f);
            int stepH = (int)(idealAtlasH * .15f);
            if (stepW == 0)
                stepW = 1;
            if (stepH == 0)
                stepH = 1;
            int numWIterations = 2;
            int steppedWidth = idealAtlasW;
            int steppedHeight = idealAtlasH;

            while (numWIterations >= 1 && steppedHeight < sqrtArea * 1000)
            {
                bool successW = false;
                numWIterations = 0;
                steppedWidth = idealAtlasW;
                while (!successW && steppedWidth < sqrtArea * 1000)
                {
                    ProbeResult pr = new ProbeResult();
                    Debug.Log("Probing h=" + steppedHeight + " w=" + steppedWidth);

                    if (ProbeSingleAtlas(imgsToAdd, steppedWidth, steppedHeight, allImageTotalArea, maxDimensionX, maxDimensionY, pr))
                    {
                        successW = true;
                        if (bestRoot == null)
                            bestRoot = pr;
                        else if (pr.GetScore(atlasMustBePowerOfTwo) > bestRoot.GetScore(atlasMustBePowerOfTwo))
                            bestRoot = pr;
                    }
                    else
                    {
                        numWIterations++;
                        steppedWidth = SetStepWidthHeight(steppedWidth, stepW, maxDimensionX);
                        Debug.Log("增加 Width h=" + steppedHeight + " w=" + steppedWidth);
                    }
                }
                steppedHeight = SetStepWidthHeight(steppedHeight, stepH, maxDimensionY);
                Debug.Log("增加 Height h=" + steppedHeight + " w=" + steppedWidth);
            }
            if (bestRoot == null)
            {
                return null;
            }

            int outW = 0;
            int outH = 0;
            if (atlasMustBePowerOfTwo)
            {
                outW = Mathf.Min(CeilToNearestPowerOfTwo(bestRoot.w), maxDimensionX);
                outH = Mathf.Min(CeilToNearestPowerOfTwo(bestRoot.h), maxDimensionY);
                if (outH < outW / 2) outH = outW / 2; //smaller dim can't be less than half larger
                if (outW < outH / 2) outW = outH / 2;
            }
            else
            {
                outW = Mathf.Min(bestRoot.w, maxDimensionX);
                outH = Mathf.Min(bestRoot.h, maxDimensionY);
            }

            bestRoot.outW = outW;
            bestRoot.outH = outH;
            Debug.Log("Best fit found: atlasW=" + outW +
                " atlasH" + outH +
                " w=" + bestRoot.w +
                " h=" + bestRoot.h +
                " efficiency=" + bestRoot.efficiency +
                " squareness=" + bestRoot.squareness +
                " fits in max dimension=" + bestRoot.largerOrEqualToMaxDim);

            //Debug.Assert(images.Count != imgsToAdd.Length, "Result images not the same lentgh as source"));

            //the atlas can be larger than the max dimension scale it if this is the case
            //int newMinSizeX = minImageSizeX;
            //int	newMinSizeY = minImageSizeY;


            List<ImageAreaInAtlas> images = new List<ImageAreaInAtlas>();
            flattenTree(bestRoot.root, images);
            images.Sort(new ImgIDComparer());
            // the atlas may be packed larger than the maxDimension. If so then the atlas needs to be scaled down to fit
            Vector2 rootWH = new Vector2(bestRoot.w, bestRoot.h);
            float padX, padY;
            int newMinSizeX, newMinSizeY;
            if (!ScaleAtlasToFitMaxDim(rootWH, images, maxDimensionX, maxDimensionY, paddings[0], minImageSizeX, minImageSizeY, masterImageSizeX, masterImageSizeY,
                ref outW, ref outH, out padX, out padY, out newMinSizeX, out newMinSizeY))
            {
                AtlasPackingResult res = new AtlasPackingResult(paddings.ToArray());
                res.rects = new Rect[images.Count];
                res.srcImgIdxs = new int[images.Count];
                res.atlasX = outW;
                res.atlasY = outH;
                res.usedW = -1;
                res.usedH = -1;
                for (int i = 0; i < images.Count; i++)
                {
                    ImageAreaInAtlas im = images[i];
                    Rect r = res.rects[i] = new Rect((float)im.x / (float)outW + padX,
                                                     (float)im.y / (float)outH + padY,
                                                     (float)im.w / (float)outW - padX * 2f,
                                                     (float)im.h / (float)outH - padY * 2f);
                    res.srcImgIdxs[i] = im.imgId;
                    Debug.Log("Image: " + i + " imgID=" + im.imgId + " x=" + r.x * outW +
                               " y=" + r.y * outH + " w=" + r.width * outW +
                               " h=" + r.height * outH + " padding=" + paddings[i]);
                }
                res.CalcUsedWidthAndHeight();
                return res;


            }
            else
            {
                Debug.Log("==================== REDOING PACKING ================");
                //root = null;
                return _GetRectsSingleAtlas(imgWidthHeights, paddings, maxDimensionX, maxDimensionY, newMinSizeX, newMinSizeY, masterImageSizeX, masterImageSizeY, recursionDepth + 1);
            }


            //Debug.Log(String.Format("Done GetRects atlasW={0} atlasH={1}", bestRoot.w, bestRoot.h));		

            //return res;			
        }

        bool ProbeSingleAtlas(ImageAreaInAtlas[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDimX, int maxAtlasDimY, ProbeResult pr)
        {
            AtalsAreaNode root = new AtalsAreaNode(NodeType.maxDim);
            root.nodeAreaRect = new AtalsAreaPixRect(0, 0, idealAtlasW, idealAtlasH);
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                AtalsAreaNode n = root.Insert(imgsToAdd[i], false);
                if (n == null)
                {
                    return false;
                }
                else if (i == imgsToAdd.Length - 1)
                {
                    int usedW = 0;
                    int usedH = 0;
                    GetExtent(root, ref usedW, ref usedH);
                    float efficiency, squareness;
                    bool fitsInMaxDim;
                    int atlasW = usedW;
                    int atlasH = usedH;
                    if (atlasMustBePowerOfTwo)
                    {
                        atlasW = Mathf.Min(CeilToNearestPowerOfTwo(usedW), maxAtlasDimX);
                        atlasH = Mathf.Min(CeilToNearestPowerOfTwo(usedH), maxAtlasDimY);
                        if (atlasH < atlasW / 2) atlasH = atlasW / 2;
                        if (atlasW < atlasH / 2) atlasW = atlasH / 2;
                        fitsInMaxDim = usedW <= maxAtlasDimX && usedH <= maxAtlasDimY;
                        float scaleW = Mathf.Max(1f, ((float)usedW) / maxAtlasDimX);
                        float scaleH = Mathf.Max(1f, ((float)usedH) / maxAtlasDimY);
                        float atlasArea = atlasW * scaleW * atlasH * scaleH; //area if we scaled it up to something large enough to contain images
                        efficiency = 1f - (atlasArea - imgArea) / atlasArea;
                        squareness = 1f; //don't care about squareness in power of two case
                    }
                    else
                    {
                        efficiency = 1f - (usedW * usedH - imgArea) / (usedW * usedH);
                        if (usedW < usedH) squareness = (float)usedW / (float)usedH;
                        else squareness = (float)usedH / (float)usedW;
                        fitsInMaxDim = usedW <= maxAtlasDimX && usedH <= maxAtlasDimY;
                    }
                    pr.Set(usedW, usedH, atlasW, atlasH, root, fitsInMaxDim, efficiency, squareness);
                    Debug.Log("Probe success efficiency w=" + usedW + " h=" + usedH + " e=" + efficiency + " sq=" + squareness + " fits=" + fitsInMaxDim);
                    return true;
                }
            }
            Debug.LogError("Should never get here.");
            return false;
        }

        int SetStepWidthHeight(int oldVal, int step, int maxDim)
        {
            if (atlasMustBePowerOfTwo && oldVal < maxDim)
            {
                return oldVal * 2;
            }
            else
            {
                int newVal = oldVal + step;
                if (newVal > maxDim && oldVal < maxDim)
                    newVal = maxDim;
                return newVal;
            }
        }

        #endregion

        #region 打包至多个图集
        //----------------- Algorithm for fitting everything into multiple Atlases
        //
        // for images being added calc area, maxW, maxH. A perfectly packed atlas will match area exactly. atlas must be at least maxH and maxW in size.
        // Sort images from big to small using either height, width or area comparer
        // 
        // If an image is bigger than maxDim, then shrink it to max size on the largest dimension
        // distribute images using the new algorithm, should never have to expand the atlas instead create new atlases as needed
        // should not need to scale atlases
        //
        AtlasPackingResult[] _GetRectsMultiAtlas(List<Vector2> imgWidthHeights, List<AtlasPadding> paddings, int maxDimensionPassedX, int maxDimensionPassedY, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY)
        {
            Debug.Log(String.Format("_GetRects numImages={0}, maxDimensionX={1}, maxDimensionY={2} minImageSizeX={3}, minImageSizeY={4}, masterImageSizeX={5}, masterImageSizeY={6}",
                                                                             imgWidthHeights.Count, maxDimensionPassedX, maxDimensionPassedY, minImageSizeX, minImageSizeY, masterImageSizeX, masterImageSizeY));
            float area = 0;
            int maxW = 0;
            int maxH = 0;
            ImageAreaInAtlas[] imgsToAdd = new ImageAreaInAtlas[imgWidthHeights.Count];
            int maxDimensionX = maxDimensionPassedX;
            int maxDimensionY = maxDimensionPassedY;
            if (atlasMustBePowerOfTwo)
            {
                maxDimensionX = RoundToNearestPositivePowerOfTwo(maxDimensionX);
                maxDimensionY = RoundToNearestPositivePowerOfTwo(maxDimensionY);
            }
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                int iw = (int)imgWidthHeights[i].x;
                int ih = (int)imgWidthHeights[i].y;

                //shrink the image so that it fits in maxDimenion if it is larger than maxDimension if atlas exceeds maxDim x maxDim then new alas will be created
                iw = Mathf.Min(iw, maxDimensionX - paddings[i].leftRight * 2);
                ih = Mathf.Min(ih, maxDimensionY - paddings[i].topBottom * 2);

                ImageAreaInAtlas im = imgsToAdd[i] = new ImageAreaInAtlas(i, iw, ih, paddings[i], minImageSizeX, minImageSizeY);
                area += im.w * im.h;
                maxW = Mathf.Max(maxW, im.w);
                maxH = Mathf.Max(maxH, im.h);
            }

            //explore the space to find a resonably efficient packing
            //int sqrtArea = (int)Mathf.Sqrt(area);
            int idealAtlasW;
            int idealAtlasH;

            if (atlasMustBePowerOfTwo)
            {
                idealAtlasH = RoundToNearestPositivePowerOfTwo(maxDimensionY);
                idealAtlasW = RoundToNearestPositivePowerOfTwo(maxDimensionX);
            }
            else
            {
                idealAtlasH = maxDimensionY;
                idealAtlasW = maxDimensionX;
            }

            if (idealAtlasW == 0) idealAtlasW = 4;
            if (idealAtlasH == 0) idealAtlasH = 4;

            ProbeResult pr = new ProbeResult();
            Array.Sort(imgsToAdd, new ImageHeightComparer());
            if (ProbeMultiAtlas(imgsToAdd, idealAtlasW, idealAtlasH, area, maxDimensionX, maxDimensionY, pr))
            {
                bestRoot = pr;
            }
            Array.Sort(imgsToAdd, new ImageWidthComparer());
            if (ProbeMultiAtlas(imgsToAdd, idealAtlasW, idealAtlasH, area, maxDimensionX, maxDimensionY, pr))
            {
                if (pr.totalAtlasArea < bestRoot.totalAtlasArea)
                {
                    bestRoot = pr;
                }
            }
            Array.Sort(imgsToAdd, new ImageAreaComparer());
            if (ProbeMultiAtlas(imgsToAdd, idealAtlasW, idealAtlasH, area, maxDimensionX, maxDimensionY, pr))
            {
                if (pr.totalAtlasArea < bestRoot.totalAtlasArea)
                {
                    bestRoot = pr;
                }
            }

            if (bestRoot == null)
            {
                return null;
            }
            Debug.Log("Best fit found: w=" + bestRoot.w + " h=" + bestRoot.h + " efficiency=" + bestRoot.efficiency + " squareness=" + bestRoot.squareness + " fits in max dimension=" + bestRoot.largerOrEqualToMaxDim);

            //the atlas can be larger than the max dimension scale it if this is the case
            //int newMinSizeX = minImageSizeX;
            //int newMinSizeY = minImageSizeY;
            List<AtlasPackingResult> rs = new List<AtlasPackingResult>();

            // find all Nodes that are an individual atlas
            List<AtalsAreaNode> atlasNodes = new List<AtalsAreaNode>();
            Stack<AtalsAreaNode> stack = new Stack<AtalsAreaNode>();
            AtalsAreaNode node = bestRoot.root;

            while (node != null)
            {
                stack.Push(node);
                node = node.child[0];
            }

            // traverse the tree collecting atlasNodes
            while (stack.Count > 0)
            {
                node = stack.Pop();
                if (node.isFullAtlas == NodeType.maxDim) atlasNodes.Add(node);
                if (node.child[1] != null)
                {
                    node = node.child[1];
                    while (node != null)
                    {
                        stack.Push(node);
                        node = node.child[0];
                    }
                }
            }

            //pack atlases so they all fit
            for (int i = 0; i < atlasNodes.Count; i++)
            {
                List<ImageAreaInAtlas> images = new List<ImageAreaInAtlas>();
                flattenTree(atlasNodes[i], images);
                Rect[] rss = new Rect[images.Count];
                int[] srcImgIdx = new int[images.Count];
                for (int j = 0; j < images.Count; j++)
                {
                    rss[j] = (new Rect(images[j].x - atlasNodes[i].nodeAreaRect.x, images[j].y, images[j].w, images[j].h));
                    srcImgIdx[j] = images[j].imgId;
                }
                AtlasPackingResult res = new AtlasPackingResult(paddings.ToArray());
                GetExtent(atlasNodes[i], ref res.usedW, ref res.usedH);
                res.usedW -= atlasNodes[i].nodeAreaRect.x; 
                int outW = atlasNodes[i].nodeAreaRect.w;
                int outH = atlasNodes[i].nodeAreaRect.h;
                if (atlasMustBePowerOfTwo)
                {
                    outW = Mathf.Min(CeilToNearestPowerOfTwo(res.usedW), atlasNodes[i].nodeAreaRect.w);
                    outH = Mathf.Min(CeilToNearestPowerOfTwo(res.usedH), atlasNodes[i].nodeAreaRect.h);
                    if (outH < outW / 2) outH = outW / 2; //smaller dim can't be less than half larger
                    if (outW < outH / 2) outW = outH / 2;
                } else
                {
                    outW = res.usedW;
                    outH = res.usedH;
                }

                res.atlasY = outH;
                res.atlasX = outW;
                
                res.rects = rss;
                res.srcImgIdxs = srcImgIdx;
                res.CalcUsedWidthAndHeight();
                rs.Add(res);
                normalizeRects(res, paddings[i]);
                Debug.Log(String.Format("Done GetRects "));
            }

            return rs.ToArray();
        }   

        bool ProbeMultiAtlas(ImageAreaInAtlas[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDimX, int maxAtlasDimY, ProbeResult pr)
        {
            int numAtlases = 0;
            AtalsAreaNode root = new AtalsAreaNode(NodeType.maxDim);
            root.nodeAreaRect = new AtalsAreaPixRect(0, 0, idealAtlasW, idealAtlasH);
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                AtalsAreaNode n = root.Insert(imgsToAdd[i], false);
                if (n == null)
                {
                    if (imgsToAdd[i].x > idealAtlasW && imgsToAdd[i].y > idealAtlasH)
                    {
                        return false;
                    }
                    else
                    {
                        // create a new root node wider than previous atlas
                        AtalsAreaNode newRoot = new AtalsAreaNode(NodeType.Container);
                        newRoot.nodeAreaRect = new AtalsAreaPixRect(0, 0, root.nodeAreaRect.w + idealAtlasW, idealAtlasH);
                        // create a new right child
                        AtalsAreaNode newRight = new AtalsAreaNode(NodeType.maxDim);
                        newRight.nodeAreaRect = new AtalsAreaPixRect(root.nodeAreaRect.w, 0, idealAtlasW, idealAtlasH);
                        newRoot.child[1] = newRight;
                        // insert root as a new left child
                        newRoot.child[0] = root;
                        root = newRoot;
                        n = root.Insert(imgsToAdd[i], false);
                        numAtlases++;
                    }
                }
            }
            pr.numAtlases = numAtlases;
            pr.root = root;
            //todo atlas may not be maxDim * maxDim. Do some checking to see what actual needed sizes are and update pr.totalArea
            pr.totalAtlasArea = numAtlases * maxAtlasDimX * maxAtlasDimY;
            Debug.Log("Probe success efficiency numAtlases=" + numAtlases + " totalArea=" + pr.totalAtlasArea);
            return true;

        }

        #endregion

        internal void GetExtent(AtalsAreaNode r, ref int x, ref int y)
        {
            if (r.img != null)
            {
                if (r.nodeAreaRect.x + r.img.w > x)
                {
                    x = r.nodeAreaRect.x + r.img.w;
                }
                if (r.nodeAreaRect.y + r.img.h > y) y = r.nodeAreaRect.y + r.img.h;
            }
            if (r.child[0] != null)
                GetExtent(r.child[0], ref x, ref y);
            if (r.child[1] != null)
                GetExtent(r.child[1], ref x, ref y);
        }

        #region Gizmos

        public void DrawGizmos()
        {
            if (bestRoot != null)
            {
                drawGizmosNode(bestRoot.root);
                Gizmos.color = Color.yellow;
                Vector3 extents = new Vector3(bestRoot.outW, -bestRoot.outH, 0);
                Vector3 pos = new Vector3(extents.x / 2, extents.y / 2, 0f);
                Gizmos.DrawWireCube(pos, extents);
            }
        }

        static void drawGizmosNode(AtalsAreaNode r)
        {
            Vector3 extents = new Vector3(r.nodeAreaRect.w, r.nodeAreaRect.h, 0);
            Vector3 pos = new Vector3(r.nodeAreaRect.x + extents.x / 2, -r.nodeAreaRect.y - extents.y / 2, 0f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(pos, extents);
            if (r.img != null)
            {
                Gizmos.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                extents = new Vector3(r.img.w, r.img.h, 0);
                pos = new Vector3(r.nodeAreaRect.x + extents.x / 2, -r.nodeAreaRect.y - extents.y / 2, 0f);
                Gizmos.DrawCube(pos, extents);
            }
            if (r.child[0] != null)
            {
                Gizmos.color = Color.red;
                drawGizmosNode(r.child[0]);
            }
            if (r.child[1] != null)
            {
                Gizmos.color = Color.green;
                drawGizmosNode(r.child[1]);
            }
        }

        #endregion

    }


}