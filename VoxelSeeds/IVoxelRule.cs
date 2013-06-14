using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    interface IVoxelRule
    {
        /// <summary>
        /// This function has to decide where to spawn new voxels.
        /// 
        /// You have to increase your resource and everything else yourself.
        /// If the rule cannot be applied return null.
        /// </summary>
        /// <param name="neighbourhood">3x3 array with the local information.
        /// The voxel in the middle is the one to which the rule is applied.
        /// So neighbourhood[1,1,1] is your voxel.</param>
        /// <returns>null if the rule does nothing and a 3x3 array otherwise.
        /// For each element which should not be changed set null in the array.
        /// Everything else will override or delete the previous voxel.
        /// To kill a voxel override it with a dead copy.</returns>
        VoxelInfo[,,] ApplyRule(VoxelInfo[,,] neighbourhood);
    }
}
