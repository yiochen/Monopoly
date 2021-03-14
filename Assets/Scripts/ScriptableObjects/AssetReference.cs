using UnityEngine;
using System.Collections.Generic;

namespace Monopoly.Client
{
    [CreateAssetMenu(fileName = "AssetRef", menuName = "Settings/AssetRef")]

    public class AssetReference : ScriptableObject
    {
        public Sprite AllDirectionRoad;
        public Sprite VerticalRoad;
        public Sprite HorizontalRoad;
        public Sprite LeftDownRoad;
        public Sprite LeftUpRoad;
        public Sprite RightDownRoad;
        public Sprite RightUpRoad;
        public Sprite TDownRoad;
        public Sprite TUpRoad;
        public Sprite TLeftRoad;
        public Sprite TRightRoad;
        public Sprite EmptyTile;

        public Sprite LandTile;
    }
}