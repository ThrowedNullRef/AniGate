﻿namespace AniGate.Core.DataAccess;

public interface IAnimeSynchronizationSession : IUnitOfWorkSession
{
    Task<List<Anime>> GetAnimesAsync();
}