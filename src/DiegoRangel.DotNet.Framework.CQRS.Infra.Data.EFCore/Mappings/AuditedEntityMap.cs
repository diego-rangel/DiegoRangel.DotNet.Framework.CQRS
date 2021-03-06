﻿using DiegoRangel.DotNet.Framework.CQRS.Domain.Core.Auditing;
using DiegoRangel.DotNet.Framework.CQRS.Domain.Core.Entities;
using DiegoRangel.DotNet.Framework.CQRS.Infra.CrossCutting.Services.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiegoRangel.DotNet.Framework.CQRS.Infra.Data.EFCore.Mappings
{
    public abstract class AuditedEntityMap<TEntity, TEntityKey, TUserKey> : EntityMap<TEntity, TEntityKey>
        where TEntity : Entity<TEntityKey>, IAudited<TEntityKey, TUserKey>
        where TUserKey : struct
    {
        public override void ConfigureEntityBuilder(EntityTypeBuilder<TEntity> builder)
        {
            ConfigureAuditedEntityBuilder(builder);

            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.LastModificationTime).IsRequired(false);
            builder.Property(x => x.CreatorUserId).IsRequired();
            builder.Property(x => x.LastModifierUserId).IsRequired(false);
        }

        public abstract void ConfigureAuditedEntityBuilder(EntityTypeBuilder<TEntity> builder);
    }
    
    public abstract class AuditedEntityMap<TEntity, TEntityKey, TUserKey, TUser> : AuditedEntityMap<TEntity, TEntityKey, TUserKey>
        where TEntity : Entity<TEntityKey>, IAudited<TEntityKey, TUserKey, TUser>
        where TUser : Entity<TUserKey>, IUser<TUserKey>
        where TUserKey : struct
    {
        public override void ConfigureAuditedEntityBuilder(EntityTypeBuilder<TEntity> builder)
        {
            ConfigureAuditedEntityWithUserBuilder(builder);

            builder.HasOne(x => x.CreatorUser).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.LastModifierUser).WithMany().OnDelete(DeleteBehavior.Restrict);
        }

        public abstract void ConfigureAuditedEntityWithUserBuilder(EntityTypeBuilder<TEntity> builder);
    }

    public abstract class AuditedEntityMap<TEntity> : AuditedEntityMap<TEntity, int, int>
        where TEntity : Entity<int>, IAudited<int, int>
    {

    }
}