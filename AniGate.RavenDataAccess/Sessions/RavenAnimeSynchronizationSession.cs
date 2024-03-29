﻿using AniGate.Core;
using AniGate.Core.DataAccess;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace AniGate.RavenDataAccess.Sessions;

public sealed class RavenAnimeSynchronizationSession : RavenUnitOfWork, IAnimeSynchronizationSession
{
    public RavenAnimeSynchronizationSession(IAsyncDocumentSession session) : base(session)
    {
    }

    public Task<List<Anime>> GetAnimesAsync() =>
        Session.Query<Anime>()
               .Where(a => a.IsWatching)
               .OrderBy(anime => anime.OriginalName)
               .ToListAsync();
}