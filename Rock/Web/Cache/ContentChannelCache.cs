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

using Rock.Cache;
using Rock.Data;
using Rock.Model;
using Rock.Security;

namespace Rock.Web.Cache
{
    /// <summary>
    /// Information about a Content channel that is required by the rendering engine.
    /// This information will be cached by the engine
    /// </summary>
    [Serializable]
    [Obsolete( "Use Rock.Cache.CacheContentChannel instead" )]
    public class ContentChannelCache : CachedModel<ContentChannel>
    {
        #region Constructors

        private ContentChannelCache()
        {
        }

        private ContentChannelCache( CacheContentChannel cacheContentChannel )
        {
            CopyFromNewCache( cacheContentChannel );
        }

        #endregion

        #region Properties

        private readonly object _obj = new object();

        /// <summary>
        /// Gets or sets the content channel type identifier.
        /// </summary>
        /// <value>
        /// The content channel type identifier.
        /// </value>
        public int ContentChannelTypeId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the icon CSS class.
        /// </summary>
        /// <value>
        /// The icon CSS class.
        /// </value>
        public string IconCssClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [requires approval].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires approval]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresApproval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether items are manually ordered or not
        /// </summary>
        /// <value>
        /// <c>true</c> if [items manually ordered]; otherwise, <c>false</c>.
        /// </value>
        public bool ItemsManuallyOrdered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether child items are manually ordered or not
        /// </summary>
        /// <value>
        /// <c>true</c> if [child items manually ordered]; otherwise, <c>false</c>.
        /// </value>
        public bool ChildItemsManuallyOrdered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable RSS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable RSS]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRss { get; set; }

        /// <summary>
        /// Gets or sets the channel URL.
        /// </summary>
        /// <value>
        /// The channel URL.
        /// </value>
        public string ChannelUrl { get; set; }

        /// <summary>
        /// Gets or sets the item URL.
        /// </summary>
        /// <value>
        /// The item URL.
        /// </value>
        public string ItemUrl { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes a feed can stay cached before refreshing it from the source.
        /// </summary>
        /// <value>
        /// The time to live.
        /// </value>
        public int? TimeToLive { get; set; }

        /// <summary>
        /// Gets or sets the type of the control to render when editing content for items of this type.
        /// </summary>
        /// <value>
        /// The type of the item control.
        /// </value>
        public ContentControlType ContentControlType { get; set; }

        /// <summary>
        /// Gets or sets the root image directory to use when the Html control type is used
        /// </summary>
        /// <value>
        /// The image root directory.
        /// </value>
        public string RootImageDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is index enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is index enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsIndexEnabled { get; set; }

        /// <summary>
        /// Gets the supported actions.
        /// </summary>
        /// <value>
        /// The supported actions.
        /// </value>
        public override Dictionary<string, string> SupportedActions
        {
            get
            {
                var supportedActions = base.SupportedActions;
                supportedActions.AddOrReplace( Authorization.APPROVE, "The roles and/or users that have access to approve channel items." );
                supportedActions.AddOrReplace( Authorization.INTERACT, "The roles and/or users that have access to intertact with the channel item." );
                return supportedActions;
            }
        }

        /// <summary>
        /// Gets the child Content Channels.
        /// </summary>
        /// <value>
        /// The child ContentChannels.
        /// </value>
        public List<ContentChannelCache> ChildContentChannels
        {
            get
            {
                var childContentChannels = new List<ContentChannelCache>();

                lock ( _obj )
                {
                    if ( childContentChannelIds == null )
                    {
                        using ( var rockContext = new RockContext() )
                        {
                            childContentChannelIds = new ContentChannelService( rockContext )
                                .GetChildContentChannels( Id )
                                .Select( g => g.Id )
                                .ToList();
                        }
                    }
                }

                if ( childContentChannelIds == null ) return childContentChannels;

                foreach ( var id in childContentChannelIds )
                {
                    var contentChannel = Read( id );
                    if ( contentChannel != null )
                    {
                        childContentChannels.Add( contentChannel );
                    }
                }

                return childContentChannels;
            }
        }
        private List<int> childContentChannelIds;

        /// <summary>
        /// Gets the parent content channels.
        /// </summary>
        /// <value>
        /// The parent content channels.
        /// </value>
        public List<ContentChannelCache> ParentContentChannels
        {
            get
            {
                var parentContentChannels = new List<ContentChannelCache>();

                lock ( _obj )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        parentContentChannelIds = new ContentChannelService( rockContext )
                            .GetParentContentChannels( Id )
                            .Select( g => g.Id )
                            .ToList();
                    }
                }

                if ( parentContentChannelIds == null ) return parentContentChannels;

                foreach ( int id in parentContentChannelIds )
                {
                    var contentChannel = Read( id );
                    if ( contentChannel != null )
                    {
                        parentContentChannels.Add( contentChannel );
                    }
                }

                return parentContentChannels;
            }

        }
        private List<int> parentContentChannelIds;

        /// <summary>
        /// Gets the parent authority.
        /// </summary>
        /// <value>
        /// The parent authority.
        /// </value>
        public override ISecured ParentAuthority
        {
            get
            {
                using ( var rockContext = new RockContext() )
                {
                    var contentChannelType = new ContentChannelTypeService( rockContext ).Get( ContentChannelTypeId );
                    return contentChannelType ?? base.ParentAuthority;
                }

            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies from model.
        /// </summary>
        /// <param name="model">The model.</param>
        public override void CopyFromModel( IEntity model )
        {
            base.CopyFromModel( model );

            if ( !( model is ContentChannel ) ) return;

            var contentChannel = (ContentChannel)model;
            ContentChannelTypeId = contentChannel.ContentChannelTypeId;
            Name = contentChannel.Name;
            Description = contentChannel.Description;
            IconCssClass = contentChannel.IconCssClass;
            RequiresApproval = contentChannel.RequiresApproval;
            ItemsManuallyOrdered = contentChannel.ItemsManuallyOrdered;
            ChildItemsManuallyOrdered = contentChannel.ChildItemsManuallyOrdered;
            EnableRss = contentChannel.EnableRss;
            ChannelUrl = contentChannel.ChannelUrl;
            ItemUrl = contentChannel.ItemUrl;
            TimeToLive = contentChannel.TimeToLive;
            ContentControlType = contentChannel.ContentControlType;
            RootImageDirectory = contentChannel.RootImageDirectory;
            IsIndexEnabled = contentChannel.IsIndexEnabled;
        }

        /// <summary>
        /// Copies properties from a new cached entity
        /// </summary>
        /// <param name="cacheEntity">The cache entity.</param>
        protected sealed override void CopyFromNewCache( IEntityCache cacheEntity )
        {
            base.CopyFromNewCache( cacheEntity );

            if ( !( cacheEntity is CacheContentChannel ) ) return;

            var contentChannel = (CacheContentChannel)cacheEntity;
            ContentChannelTypeId = contentChannel.ContentChannelTypeId;
            Name = contentChannel.Name;
            Description = contentChannel.Description;
            IconCssClass = contentChannel.IconCssClass;
            RequiresApproval = contentChannel.RequiresApproval;
            ItemsManuallyOrdered = contentChannel.ItemsManuallyOrdered;
            ChildItemsManuallyOrdered = contentChannel.ChildItemsManuallyOrdered;
            EnableRss = contentChannel.EnableRss;
            ChannelUrl = contentChannel.ChannelUrl;
            ItemUrl = contentChannel.ItemUrl;
            TimeToLive = contentChannel.TimeToLive;
            ContentControlType = contentChannel.ContentControlType;
            RootImageDirectory = contentChannel.RootImageDirectory;
            IsIndexEnabled = contentChannel.IsIndexEnabled;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Performs an implicit conversion from <see cref="ContentChannelCache"/> to <see cref="CacheContentChannel"/>.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator CacheContentChannel( ContentChannelCache c )
        {
            return CacheContentChannel.Get( c.Id );
        }

        /// <summary>
        /// Returns content channel object from cache.  If content channel does not already exist in cache, it
        /// will be read and added to cache
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        public static ContentChannelCache Read( int id, RockContext rockContext = null )
        {
            return new ContentChannelCache( CacheContentChannel.Get( id, rockContext ) );
        }

        /// <summary>
        /// Reads the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        public static ContentChannelCache Read( Guid guid, RockContext rockContext = null )
        {
            return new ContentChannelCache( CacheContentChannel.Get( guid, rockContext ) );
        }

        /// <summary>
        /// Adds content channel model to cache, and returns cached object
        /// </summary>
        /// <param name="contentChannel"></param>
        /// <returns></returns>
        public static ContentChannelCache Read( ContentChannel contentChannel )
        {
            return new ContentChannelCache( CacheContentChannel.Get( contentChannel ) );
        }

        /// <summary>
        /// Returns all content channels
        /// </summary>
        /// <returns></returns>
        public static List<ContentChannelCache> All()
        {
            var contentChannels = new List<ContentChannelCache>();

            var cacheContentChannels = CacheContentChannel.All();
            if ( cacheContentChannels == null ) return contentChannels;

            foreach ( var cacheContentChannel in cacheContentChannels )
            {
                contentChannels.Add( new ContentChannelCache( cacheContentChannel ) );
            }

            return contentChannels;
        }

        /// <summary>
        /// Removes content channel from cache
        /// </summary>
        /// <param name="id"></param>
        public static void Flush( int id )
        {
            CacheContentChannel.Remove( id );
        }

        #endregion
    }
}