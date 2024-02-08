// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Xml.Serialization;

namespace Voting.Stimmregister.Domain.Models.Utils;

[Serializable]
[XmlRoot("CountryCollection")]
public class CountryXmlRootModel<TElement>
{
    [XmlArray("countries")]
    [XmlArrayItem("country")]
    public TElement[]? Country { get; set; }
}
