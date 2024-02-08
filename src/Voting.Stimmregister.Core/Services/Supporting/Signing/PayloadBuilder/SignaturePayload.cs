// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Diagnostics.CodeAnalysis;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

[SuppressMessage(
    "StyleCop.CSharp.NamingRules",
    "SA1313:Parameter names should begin with lower-case letter",
    Justification = "It is a primary ctor parameter and therefore should have the naming rules of properties")]
public readonly record struct SignaturePayload(
    AsymmetricKeyConfig Config,
    string TypeName,
    byte Version,
    byte[] Payload);
