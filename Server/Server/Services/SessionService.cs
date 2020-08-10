using System;
using System.Collections.Generic;

using Autofac;

using Server.Dto;
using Server.Models;

namespace Server.Services
{
    public class SessionService
    {
        private const int SESSION_DURATION = 3600 * 24;
        private static readonly IDictionary<int, Session> Sessions = new Dictionary<int, Session>();
        private static readonly HashingService Hasher = DependencyHolder.ServiceDependencies.Resolve<HashingService>();

        private string GenerateToken()
        {
            string seed = Guid.NewGuid().ToString();
            return Hasher.GetHash(seed);
        }

        public Session CreateSessionFor(int accountId)
        {
            if (Sessions.ContainsKey(accountId))
                Sessions.Remove(accountId);

            string token = GenerateToken();
            DateTime expires = DateTime.Now.AddSeconds(SESSION_DURATION);

            Session session = new Session(accountId, token, expires);
            Sessions.Add(accountId, session);
            return session;
        }

        public bool IsActive(int accountId)
        {
            return Sessions.ContainsKey(accountId) && Sessions[accountId].Expires > DateTime.Now;
        }

        public Session GetByAccountId(int accountId)
        {
            return Sessions[accountId];
        }

        public void CheckSession(SessionDto sessionDto)
        {
            if (sessionDto == null || !Sessions.ContainsKey(sessionDto.UserId.GetValueOrDefault()))
                throw new UnauthorizedAccessException("Session not found");

            string originalTokenSalted = Hasher.GetHash(Sessions[sessionDto.UserId.GetValueOrDefault()].Token + sessionDto.Salt);
            if (originalTokenSalted.ToUpper() != sessionDto.SessionTokenSalted.ToUpper())
                throw new UnauthorizedAccessException("Wrong session token");

            if (Sessions[sessionDto.UserId.GetValueOrDefault()].Expires < DateTime.Now)
            {
                Sessions.Remove(sessionDto.UserId.GetValueOrDefault());
                throw new UnauthorizedAccessException("Session expired");
            }

            //Sessions[sessionDto.UserId.GetValueOrDefault()].Expires.AddSeconds(SESSION_DURATION);
        }

        public void TerminateSession(SessionDto sessionDto)
        {
            CheckSession(sessionDto);
            Sessions.Remove(sessionDto.UserId.GetValueOrDefault());
        }
    }
}