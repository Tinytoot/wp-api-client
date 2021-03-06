﻿// -----------------------------------------------------------------------
// <copyright file="MusicClient.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone
{
    /// <summary>
    ///   The Nokia Music API client
    /// </summary>
    public sealed class MusicClient : IMusicClientSettings, IMusicClient
    {
        internal const int DefaultItemsPerPage = 10;
        internal const int DefaultStartIndex = 0;

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class,
        ///   using the RegionInfo settings to locate the user.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        /// <param name="appCode"> The App Code obtained from api.developer.nokia.com </param>
        public MusicClient(string appId, string appCode)
            : this(
                appId,
                appCode,
                RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower(),
                new ApiRequestHandler(new ApiUriBuilder()))
        {
            this.CountryCodeBasedOnRegionInfo = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        /// <param name="appCode"> The App Code obtained from api.developer.nokia.com </param>
        /// <param name="countryCode"> The country code. </param>
        public MusicClient(string appId, string appCode, string countryCode)
            : this(appId, appCode, countryCode, new ApiRequestHandler(new ApiUriBuilder()))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        /// <param name="appCode"> The App Code obtained from api.developer.nokia.com </param>
        /// <param name="requestHandler"> The request handler. </param>
        /// <remarks>
        ///   Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClient(string appId, string appCode, IApiRequestHandler requestHandler)
            : this(appId, appCode, RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower(), requestHandler)
        {
            this.CountryCodeBasedOnRegionInfo = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        /// <param name="appCode"> The App Code obtained from api.developer.nokia.com </param>
        /// <param name="countryCode"> The country code. </param>
        /// <param name="requestHandler"> The request handler. </param>
        /// <remarks>
        ///   Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClient(string appId, string appCode, string countryCode, IApiRequestHandler requestHandler)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appCode))
            {
                throw new ApiCredentialsRequiredException();
            }

            this.AppId = appId;
            this.AppCode = appCode;
            this.RequestHandler = requestHandler;

            if (this.ValidateCountryCode(countryCode))
            {
                this.CountryCode = countryCode.ToLowerInvariant();
            }
            else
            {
                throw new InvalidCountryCodeException();
            }
        }

        #region IMusicClientSettings Members

        /// <summary>
        /// Gets a value indicating whether the country code was based on region info.
        /// </summary>
        /// <value>
        /// <c>true</c> if the country code was based on region info; otherwise, <c>false</c>.
        /// </value>
        public bool CountryCodeBasedOnRegionInfo { get; private set; }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        /// <value>
        /// The app id.
        /// </value>
        public string AppId { get; private set; }

        /// <summary>
        /// Gets the app code.
        /// </summary>
        /// <value>
        /// The app code.
        /// </value>
        public string AppCode { get; private set; }

        /// <summary>
        /// Gets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; private set; }

        #endregion

        /// <summary>
        /// Gets the request handler in use for testing purposes.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler { get; private set; }

        #region IMusicClient Members
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void SearchArtists(Action<ListResponse<Artist>> callback, string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SearchArtistsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopArtists(Action<ListResponse<Artist>> callback, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<TopArtistsCommand>();
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopArtistsForGenre(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<TopArtistsForGenreCommand>();
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="genre">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopArtistsForGenre(Action<ListResponse<Artist>> callback, Genre genre, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre", "genre cannot be null");
            }

            this.GetTopArtistsForGenre(callback, genre.Id, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetSimilarArtists(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SimilarArtistsCommand>();
            cmd.ArtistId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetSimilarArtists(Action<ListResponse<Artist>> callback, Artist artist, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist", "Artist cannot be null");
            }

            this.GetSimilarArtists(callback, artist.Id, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The artist id.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetArtistProducts(Action<ListResponse<Product>> callback, string id, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<ArtistProductsCommand>();
            cmd.ArtistId = id;
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetArtistProducts(Action<ListResponse<Product>> callback, Artist artist, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist", "Artist cannot be null");
            }

            this.GetArtistProducts(callback, artist.Id, category, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopProducts(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<TopProductsCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetNewReleases(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<NewReleasesCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        public void GetGenres(Action<ListResponse<Genre>> callback)
        {
            var cmd = this.Create<GenresCommand>();
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Searches Nokia Music
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void Search(Action<ListResponse<MusicItem>> callback, string searchTerm, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SearchCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetMixGroups(Action<ListResponse<MixGroup>> callback, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<MixGroupsCommand>();
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The mix group id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetMixes(Action<ListResponse<Mix>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<MixesCommand>();
            cmd.MixGroupId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="group">The mix group.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetMixes(Action<ListResponse<Mix>> callback, MixGroup group, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group", "group cannot be null");
            }

            this.GetMixes(callback, group.Id, startIndex, itemsPerPage);
        }
        #endregion

        /// <summary>
        /// Creates a command to execute
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns>A Command to call</returns>
        private TCommand Create<TCommand>() where TCommand : MusicClientCommand, new()
        {
            return new TCommand
            {
                MusicClientSettings = this,
                RequestHandler = this.RequestHandler
            };
        }

        /// <summary>
        ///   Validates a country code.
        /// </summary>
        /// <param name="countryCode"> The country code. </param>
        /// <returns> A Boolean indicating that the country code is valid </returns>
        private bool ValidateCountryCode(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                return countryCode.Length == 2;
            }
            else
            {
                return false;
            }
        }
    }
}