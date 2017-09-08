﻿//----------------------------------------------------------------------------------------------
// <copyright file="NotSpecification.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
// ===================================================================================
// Microsoft Developer & Platform Evangelism
// ===================================================================================
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license,
// http://microsoftnlayerapp.codeplex.com/license
// ===================================================================================
namespace Hexa.Core.Domain.Specification
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// NotEspecification convert a original
    /// specification with NOT logic operator
    /// </summary>
    /// <typeparam name="TEntity">Type of element for this specificaiton</typeparam>
    public class NotSpecification<TEntity> : Specification<TEntity>
        where TEntity : class
    {
        private readonly Expression<Func<TEntity, bool>> originalCriteria;

        /// <summary>
        /// Constructor for NotSpecificaiton
        /// </summary>
        /// <param name="originalSpecification">Original specification</param>
        public NotSpecification(ISpecification<TEntity> originalSpecification)
        {
            if (originalSpecification == null)
            {
                throw new ArgumentNullException("originalSpecification");
            }

            this.originalCriteria = originalSpecification.SatisfiedBy();
        }

        /// <summary>
        /// Constructor for NotSpecification
        /// </summary>
        /// <param name="originalSpecification">Original specificaiton</param>
        public NotSpecification(Expression<Func<TEntity, bool>> originalSpecification)
        {
            if (originalSpecification == null)
            {
                throw new ArgumentNullException("originalSpecification");
            }

            this.originalCriteria = originalSpecification;
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.Specification.ISpecification{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Hexa.Core.Domain.Specification.ISpecification{TEntity}"/></returns>
        public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.Not(this.originalCriteria.Body),
                this.originalCriteria.Parameters.Single());
        }
    }
}