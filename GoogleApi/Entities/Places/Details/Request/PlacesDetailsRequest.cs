﻿using System;
using System.Collections.Generic;
using System.Linq;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Common.Enums.Extensions;
using GoogleApi.Entities.Common.Extensions;
using GoogleApi.Entities.Places.Details.Request.Enums;

namespace GoogleApi.Entities.Places.Details.Request
{
    /// <summary>
    /// Places Details Request.
    /// </summary>
    public class PlacesDetailsRequest : BasePlacesRequest
    {
        /// <summary>
        /// Base Url.
        /// </summary>
        protected internal override string BaseUrl => $"{base.BaseUrl}details/json";

        /// <summary>
        /// A textual identifier that uniquely identifies a place, returned from a Place Search.
        /// </summary>
        public virtual string PlaceId { get; set; }

        /// <summary>
        /// The region code, specified as a ccTLD ("top-level domain") two-character value.
        /// Most ccTLD codes are identical to ISO 3166-1 codes, with some notable exceptions.
        /// For example, the United Kingdom's ccTLD is "uk" (.co.uk) while its ISO 3166-1 code is "gb"
        /// (technically for the entity of "The United Kingdom of Great Britain and Northern Ireland").
        /// </summary>
        public virtual string Region { get; set; }

        /// <summary>
        /// A random string which identifies an autocomplete session for billing purposes.
        /// The session begins when the user starts typing a query, and concludes when they select a place and a
        /// call to Place Details is made.Each session can have multiple queries,
        /// followed by one place selection.The API key(s) used for each request within a session must belong to the same Google Cloud Console project.
        /// Once a session has concluded, the token is no longer valid; your app must generate a fresh token for each session.
        /// If the sessiontoken parameter is omitted, or if you reuse a session token,
        /// the session is charged as if no session token was provided (each request is billed separately).
        /// </summary>
        public virtual string SessionToken { get; set; }

        /// <summary>
        /// Language (optional) — The language code, indicating in which language the results should be returned, if possible. 
        /// See the list of supported languages and their codes: https://developers.google.com/maps/faq#languagesupport
        /// Note that we often update supported languages so this list may not be exhaustive.
        /// </summary>
        public virtual Language Language { get; set; } = Language.English;

        /// <summary>
        /// Fields (optional).
        /// Defaults to 'Basic'.
        /// One or more fields, specifying the types of place data to return, separated by a comma.
        /// Fields correspond to Place Details results, and are divided into three billing categories: Basic, Contact, and Atmosphere.
        /// Basic fields are billed at base rate, and incur no additional charges.Contact and Atmosphere fields are billed at a higher rate.
        /// See the pricing sheet for more information. Attributions (html_attributions) are always returned with every call, regardless of whether it has been requested.
        /// </summary>
        public virtual FieldTypes Fields { get; set; } = FieldTypes.Basic;

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            if (string.IsNullOrWhiteSpace(this.PlaceId))
                throw new ArgumentException($"'{nameof(this.PlaceId)}' is required");

            parameters.Add("placeid", this.PlaceId);
            parameters.Add("language", this.Language.ToCode());

            var fields = Enum.GetValues(typeof(FieldTypes))
                .Cast<FieldTypes>()
                .Where(x => this.Fields.HasFlag(x) && x != FieldTypes.Basic && x != FieldTypes.Contact && x != FieldTypes.Atmosphere)
                .Aggregate(string.Empty, (current, x) => $"{current}{x.ToString().ToLowerInvariant()},");

            parameters.Add("fields", fields.EndsWith(",") ? fields.Substring(0, fields.Length - 1) : fields);

            if (!string.IsNullOrEmpty(this.Region))
                parameters.Add("region", this.Region);

            if (!string.IsNullOrEmpty(this.SessionToken))
                parameters.Add("sessiontoken", this.SessionToken);

            return parameters;
        }
    }
}