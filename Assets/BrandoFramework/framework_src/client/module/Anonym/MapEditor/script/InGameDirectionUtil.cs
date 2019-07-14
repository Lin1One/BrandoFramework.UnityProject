using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anonym.Util
{
    using Isometric;

    public static class InGameDirectionUtil
    {
        const int way = 8;

        static int To8WayDir(int iDir)
        {
            iDir = iDir % way;
            return iDir == 0 ? way : iDir;
        }
        public static InGameDirection[] Get(this InGameDirection dir, bool self = false, bool sides = false, bool rights = false, bool sides_of_opposite = false, bool opposite = false)
        {
            // 8 Way directions
            // 1(Right) ~ 8(TopRight) loop

            int iDir = (int)dir;
            List<int> result = new List<int>();
            if (self)
                result.Add(To8WayDir(iDir));

            if (sides)
            {
                result.Add(To8WayDir(iDir + 1));
                result.Add(To8WayDir(iDir + way - 1));
            }

            if (rights)
            {
                result.Add(To8WayDir(iDir + 2));
                result.Add(To8WayDir(iDir + way - 2));
            }

            if (sides_of_opposite)
            {
                result.Add(To8WayDir(iDir + 3));
                result.Add(To8WayDir(iDir + way - 3));
            }

            if (opposite)
                result.Add(To8WayDir(iDir + 4));

            return result.Select(i => (InGameDirection)i).ToArray();
        }
    }
}