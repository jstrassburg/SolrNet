﻿#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl
{
	/// <summary>
	/// Parses documents from query response
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrDocumentResponseParser<T> : ISolrDocumentResponseParser<T>
	{
		private readonly IReadOnlyMappingManager mappingManager;
		private readonly ISolrDocumentPropertyVisitor propVisitor;
		private readonly ISolrDocumentActivator<T> activator;

		public SolrDocumentResponseParser(IReadOnlyMappingManager mappingManager, ISolrDocumentPropertyVisitor propVisitor, ISolrDocumentActivator<T> activator)
		{
			this.mappingManager = mappingManager;
			this.propVisitor = propVisitor;
			this.activator = activator;
		}

		/// <summary>
		/// Parses documents results
		/// </summary>
		/// <param name="parentNode"></param>
		/// <returns></returns>
		public IList<T> ParseResults(XElement parentNode)
		{
			var results = new List<T>();
			if (parentNode == null)
				return results;
			var nodes = parentNode.Elements("doc");
			foreach (var docNode in nodes)
			{
				results.AddRange(ParseDocument(docNode));
			}
			return results;
		}

		/// <summary>
		/// Builds a document from the corresponding response xml node
		/// </summary>
		/// <param name="node">response xml node</param>
		/// <param name="fields">document fields</param>
		/// <returns>populated document(s)</returns>
		public IEnumerable<T> ParseDocument(XElement node)
		{
			IList<T> docs = new List<T>();
			IList<XElement> children = new List<XElement>();

			var doc = activator.Create();

			foreach (var field in node.Elements())
			{
				if (field.HasElements &&
							 string.Equals(field.Name.LocalName, "doc", System.StringComparison.OrdinalIgnoreCase))
				{
					children.Add(field);
				}
				else
				{
					string fieldName = field.Attribute("name").Value;
					propVisitor.Visit(doc, fieldName, field);
				}
			}

			if (children.Count > 0) {
				foreach (var child in children)
				{
					var newDoc = ShallowClone.Clone(doc);
					foreach (var field in child.Elements())
					{
						string fieldName = field.Attribute("name").Value;
						propVisitor.Visit(newDoc, fieldName, field);
					}
					docs.Add(newDoc);
				}
			}
			else
			{
				docs.Add(doc);
			}

			return docs;
		}
	}
}
