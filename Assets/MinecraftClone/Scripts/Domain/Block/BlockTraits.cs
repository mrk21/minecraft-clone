using System;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Block
{
    public struct BlockTraits : IValueObject<BlockTraits>
    {
        public enum MatterTypeEnum
        {
            Solid,
            Fluid,
            Void,
        }

        public enum TransparencyTypeEnum
        {
            Opaque,
            Translucent,
            Transparent,
        }

        public enum CollisionTypeEnum
        {
            Collision,
            NonCollision,
        }

        private MatterTypeEnum matterType;
        private TransparencyTypeEnum transparencyType;
        private CollisionTypeEnum collisionType;

        public BlockTraits(
            MatterTypeEnum matterType,
            TransparencyTypeEnum transparencyType,
            CollisionTypeEnum collisionType
        )
        {
            this.matterType = matterType;
            this.transparencyType = transparencyType;
            this.collisionType = collisionType;
        }

        public MatterTypeEnum MatterType { get { return matterType; } }
        public TransparencyTypeEnum TransparencyType { get { return transparencyType; } }
        public CollisionTypeEnum CollisionType { get { return collisionType; } }

        public bool IsBreakable()
        {
            return MatterType == MatterTypeEnum.Solid;
        }

        public bool IsReplaceable()
        {
            return MatterType == MatterTypeEnum.Fluid
                || MatterType == MatterTypeEnum.Void;
        }

        public bool IsTransparent()
        {
            return TransparencyType == TransparencyTypeEnum.Translucent
                || TransparencyType == TransparencyTypeEnum.Transparent;
        }

        public static readonly BlockTraits SolidBlock = new BlockTraits(
            matterType: MatterTypeEnum.Solid,
            transparencyType: TransparencyTypeEnum.Opaque,
            collisionType: CollisionTypeEnum.Collision
        );

        public static readonly BlockTraits TransparentSolidBlock = new BlockTraits(
            matterType: MatterTypeEnum.Solid,
            transparencyType: TransparencyTypeEnum.Translucent,
            collisionType: CollisionTypeEnum.Collision
        );

        public static readonly BlockTraits FluidBlock = new BlockTraits(
            matterType: MatterTypeEnum.Fluid,
            transparencyType: TransparencyTypeEnum.Translucent,
            collisionType: CollisionTypeEnum.NonCollision
        );

        public static readonly BlockTraits VoidBlock = new BlockTraits(
            matterType: MatterTypeEnum.Void,
            transparencyType: TransparencyTypeEnum.Transparent,
            collisionType: CollisionTypeEnum.NonCollision
        );
    }
}
