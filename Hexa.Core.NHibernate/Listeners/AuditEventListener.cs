﻿//----------------------------------------------------------------------------------------------
// <copyright file="AuditEventListener.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Hexa.Core;
    using NHibernate.Cfg;
    using NHibernate.Event;
    using NHibernate.Event.Default;
    using NHibernate.Persister.Entity;
    using Security;

    public class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            var auditable = @event.Entity as IAuditableEntity;
            if (auditable == null)
            {
                return false;
            }

            string userUniqueId = string.Empty;

            var user = ApplicationContext.User;
            if (user != null)
            {
                var identity = ApplicationContext.User.Identity as ICoreIdentity;
                if (identity != null)
                {
                    userUniqueId = identity.Id;
                }
            }

            DateTime createdAt = DateTime.Now;

            this.Set(@event.Persister, @event.State, "CreatedBy", userUniqueId);
            this.Set(@event.Persister, @event.State, "UpdatedBy", userUniqueId);
            this.Set(@event.Persister, @event.State, "CreatedAt", createdAt);
            this.Set(@event.Persister, @event.State, "UpdatedAt", createdAt);

            auditable.CreatedBy = userUniqueId;
            auditable.UpdatedBy = userUniqueId;
            auditable.CreatedAt = createdAt;
            auditable.UpdatedAt = createdAt;

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var auditable = @event.Entity as IAuditableEntity;
            if (auditable == null)
            {
                return false;
            }

            string userUniqueId = string.Empty;

            var user = ApplicationContext.User;
            if (user != null)
            {
                var identity = ApplicationContext.User.Identity as ICoreIdentity;
                if (identity != null)
                {
                    userUniqueId = identity.Id;
                }
            }

            DateTime updatedAt = DateTime.Now;

            this.Set(@event.Persister, @event.State, "UpdatedBy", userUniqueId);
            this.Set(@event.Persister, @event.State, "UpdatedAt", updatedAt);
            auditable.UpdatedBy = userUniqueId;
            auditable.UpdatedAt = updatedAt;

            return false;
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
            {
                return;
            }

            state[index] = value;
        }

        public static void AppendTo(Configuration configuration)
        {
            configuration.SetListeners(
                ListenerType.PreInsert,
                configuration.EventListeners.PreInsertEventListeners.Concat(new IPreInsertEventListener[] { new AuditEventListener() }).ToArray());

            configuration.SetListeners(
                ListenerType.PreUpdate,
                configuration.EventListeners.PreUpdateEventListeners.Concat(new IPreUpdateEventListener[] { new AuditEventListener() }).ToArray());
        }
    }
}