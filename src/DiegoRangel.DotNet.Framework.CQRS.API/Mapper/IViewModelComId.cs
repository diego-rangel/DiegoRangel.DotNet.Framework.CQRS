﻿using DiegoRangel.DotNet.Framework.CQRS.Domain.Core.Entities;

namespace DiegoRangel.DotNet.Framework.CQRS.API.Mapper
{
    public interface IViewModelWithId<T, K> : IViewModel<T>
        where T : class, IEntity<K>
    {
        K Id { get; set; }
    }
    public interface IViewModelWithId<T> : IViewModelWithId<T, int>
        where T : class, IEntity<int>
    {

    }
}
