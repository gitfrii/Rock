﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;

using Rock.Data;
using Rock.Cache;

namespace Rock.Model
{
    /// <summary>
    /// Service/Data access class for <see cref="Rock.Model.FinancialBatch"/> entity objects.
    /// </summary>
    public partial class FinancialBatchService
    {
        /// <summary>
        /// Gets the specified name prefix.
        /// </summary>
        /// <param name="namePrefix">The name prefix.</param>
        /// <param name="currencyType">Type of the currency.</param>
        /// <param name="creditCardType">Type of the credit card.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <param name="batchTimeOffset">The batch time offset.</param>
        /// <param name="batches">The batches.</param>
        /// <returns></returns>
        [Obsolete]
        public FinancialBatch Get( string namePrefix, Web.Cache.DefinedValueCache currencyType, Web.Cache.DefinedValueCache creditCardType,
            DateTime transactionDate, TimeSpan batchTimeOffset, List<FinancialBatch> batches = null )
        {
            return Get( namePrefix, string.Empty, currencyType, creditCardType, transactionDate, batchTimeOffset, batches );
        }

        /// <summary>
        /// Gets the specified name prefix.
        /// </summary>
        /// <param name="namePrefix">The name prefix.</param>
        /// <param name="currencyType">Type of the currency.</param>
        /// <param name="creditCardType">Type of the credit card.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <param name="batchTimeOffset">The batch time offset.</param>
        /// <param name="batches">The batches.</param>
        /// <returns></returns>
        public FinancialBatch Get( string namePrefix, CacheDefinedValue currencyType, CacheDefinedValue creditCardType,
            DateTime transactionDate, TimeSpan batchTimeOffset, List<FinancialBatch> batches = null )
        {
            return Get( namePrefix, string.Empty, currencyType, creditCardType, transactionDate, batchTimeOffset, batches );
        }

        /// <summary>
        /// Gets the specified name prefix.
        /// </summary>
        /// <param name="namePrefix">The name prefix.</param>
        /// <param name="nameSuffix">The name suffix.</param>
        /// <param name="currencyType">Type of the currency.</param>
        /// <param name="creditCardType">Type of the credit card.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <param name="batchTimeOffset">The batch time offset.</param>
        /// <param name="batches">The batches.</param>
        /// <returns></returns>
        [Obsolete]
        public FinancialBatch Get( string namePrefix, string nameSuffix, Web.Cache.DefinedValueCache currencyType, Web.Cache.DefinedValueCache creditCardType,
            DateTime transactionDate, TimeSpan batchTimeOffset, List<FinancialBatch> batches = null )
        {
            // Use the credit card type's batch name suffix, or if that doesn't exist, use the currency type value
            string ccSuffix = string.Empty;
            
            if (creditCardType != null )
            {
                ccSuffix = creditCardType.GetAttributeValue( "BatchNameSuffix" );
                if ( string.IsNullOrWhiteSpace( ccSuffix ) )
                {
                    ccSuffix = creditCardType.Value;
                }
            }

            if ( string.IsNullOrWhiteSpace( ccSuffix ) && currencyType != null )
            {
                ccSuffix = currencyType.Value;
            }

            string batchName = namePrefix.Trim() + ( string.IsNullOrWhiteSpace( ccSuffix ) ? "" : " " + ccSuffix ) + nameSuffix;

            return GetByNameAndDate( batchName, transactionDate, batchTimeOffset, batches );
        }

        /// <summary>
        /// Gets the specified name prefix.
        /// </summary>
        /// <param name="namePrefix">The name prefix.</param>
        /// <param name="nameSuffix">The name suffix.</param>
        /// <param name="currencyType">Type of the currency.</param>
        /// <param name="creditCardType">Type of the credit card.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <param name="batchTimeOffset">The batch time offset.</param>
        /// <param name="batches">The batches.</param>
        /// <returns></returns>
        public FinancialBatch Get( string namePrefix, string nameSuffix, CacheDefinedValue currencyType, CacheDefinedValue creditCardType,
            DateTime transactionDate, TimeSpan batchTimeOffset, List<FinancialBatch> batches = null )
        {
            // Use the credit card type's batch name suffix, or if that doesn't exist, use the currency type value
            string ccSuffix = string.Empty;

            if ( creditCardType != null )
            {
                ccSuffix = creditCardType.GetAttributeValue( "BatchNameSuffix" );
                if ( string.IsNullOrWhiteSpace( ccSuffix ) )
                {
                    ccSuffix = creditCardType.Value;
                }
            }

            if ( string.IsNullOrWhiteSpace( ccSuffix ) && currencyType != null )
            {
                ccSuffix = currencyType.Value;
            }

            string batchName = namePrefix.Trim() + ( string.IsNullOrWhiteSpace( ccSuffix ) ? "" : " " + ccSuffix ) + nameSuffix;

            return GetByNameAndDate( batchName, transactionDate, batchTimeOffset, batches );
        }

        /// <summary>
        /// Gets the by name and date.
        /// </summary>
        /// <param name="batchName">Name of the batch.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <param name="batchTimeOffset">The batch time offset.</param>
        /// <param name="batches">The batches.</param>
        /// <returns></returns>
        public FinancialBatch GetByNameAndDate ( string batchName, DateTime transactionDate, TimeSpan batchTimeOffset, List<FinancialBatch> batches = null )
        {
            FinancialBatch batch = null;

            // If a list of batches was passed, search those first
            if ( batches != null )
            {
                batch = batches
                    .Where( b =>
                        b.Status == BatchStatus.Open &&
                        b.BatchStartDateTime <= transactionDate &&
                        b.BatchEndDateTime > transactionDate &&
                        b.Name == batchName )
                    .OrderByDescending( b => b.BatchStartDateTime )
                    .FirstOrDefault();

                if ( batch != null )
                {
                    return batch;
                }
            }

            // If batch was not found in existing list, search database
            batch = Queryable()
                .Where( b =>
                    b.Status == BatchStatus.Open &&
                    b.BatchStartDateTime <= transactionDate &&
                    b.BatchEndDateTime > transactionDate &&
                    b.Name == batchName )
                .OrderByDescending( b => b.BatchStartDateTime )
                .FirstOrDefault();

            // If still no batch, create a new one
            if ( batch == null )
            {
                batch = new FinancialBatch();
                batch.Guid = Guid.NewGuid();
                batch.Name = batchName;
                batch.Status = BatchStatus.Open;

                var batchStartDateTime = transactionDate.Date.Add( batchTimeOffset );
                if ( batchStartDateTime > transactionDate )
                {
                    batchStartDateTime = batchStartDateTime.AddDays( -1 );
                }
                batch.BatchStartDateTime = batchStartDateTime;
                batch.BatchEndDateTime = batchStartDateTime.AddDays( 1 );

                batch.ControlAmount = 0;
                Add( batch );
            }

            // Add the batch to the list
            if ( batches != null )
            {
                batches.Add( batch );
            }

            return batch;
        }
    }
}