using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CretNet.Tests.Component;

public class CnGridAndAvatarTests : BunitContext
{
    [Fact]
    public void Grid_ExpansionAndConditionalColumns_PreserveDetailAndColumnLifecycle()
    {
        var cut = Render<GridFixture>();
        cut.WaitForAssertion(() => cut.FindAll("tbody tr").Count.ShouldBe(1));
        cut.Find("#expand").Click();
        cut.Find(".cn-grid-row-detail").TextContent.ShouldContain("Units of Stock");
        cut.Find(".cn-grid-row-detail td").GetAttribute("colspan").ShouldBe("1");
        cut.Find("#column").Click();
        cut.WaitForAssertion(() => cut.FindAll("th").Count.ShouldBe(2));
        cut.Find(".cn-grid-row-detail td").GetAttribute("colspan").ShouldBe("2");
        cut.Find("#column").Click();
        cut.WaitForAssertion(() => cut.FindAll("th").Count.ShouldBe(1));
        cut.Find("#expand").Click();
        cut.FindAll(".cn-grid-row-detail").ShouldBeEmpty();
    }

    [Fact]
    public async Task AvatarSource_SharesRequests_AndUpdatesAfterHostSave()
    {
        var source = new PhotoSource();
        var partyId = Guid.NewGuid();
        Services.AddSingleton<ICnAvatarSource>(source);
        var first = Render<CnAvatar>(p => p.Add(x => x.PartyId, partyId).Add(x => x.Name, "Ada Lovelace"));
        var second = Render<CnAvatar>(p => p.Add(x => x.PartyId, partyId).Add(x => x.Name, "Ada Lovelace"));
        source.Reads.ShouldBe(1);
        first.Markup.ShouldContain("AL");
        await first.InvokeAsync(() => source.Save(partyId, "data:image/png;base64,cGhvdG8="));
        first.WaitForAssertion(() => first.Find("img").GetAttribute("src").ShouldBe("data:image/png;base64,cGhvdG8="));
        second.WaitForAssertion(() => second.Find("img").GetAttribute("src").ShouldBe("data:image/png;base64,cGhvdG8="));
        source.Reads.ShouldBe(1);
        await first.InvokeAsync(() => source.Invalidate(partyId));
        source.Reads.ShouldBe(2);
    }

    [Fact]
    public void Avatar_WithoutPhotoSource_RendersInitials()
    {
        var cut = Render<CnAvatar>(p => p.Add(x => x.PartyId, Guid.NewGuid()).Add(x => x.Name, "Ada Lovelace"));
        cut.Markup.ShouldContain("AL");
        cut.FindAll("img").ShouldBeEmpty();
    }

    private sealed class PhotoSource : CnAvatarSource
    {
        public int Reads { get; private set; }
        protected override Task<string?> LoadPartyAvatarUrlAsync(Guid partyId)
        {
            Reads++;
            return Task.FromResult<string?>(null);
        }
        public void Save(Guid partyId, string url) => SetPartyAvatarUrl(partyId, url);
    }

    [Fact]
    public async Task Avatar_UploadDuringLookup_DoesNotShowTheStalePhoto()
    {
        var source = new DeferredPhotoSource();
        Services.AddSingleton<ICnAvatarSource>(source);
        var partyId = Guid.NewGuid();
        var avatar = Render<CnAvatar>(p => p.Add(x => x.PartyId, partyId).Add(x => x.Name, "Ada"));
        await avatar.InvokeAsync(() => source.Save(partyId, "new.png"));
        avatar.WaitForAssertion(() => avatar.Find("img").GetAttribute("src").ShouldBe("new.png"));
        await avatar.InvokeAsync(() => source.Request.SetResult("old.png"));
        avatar.WaitForAssertion(() => avatar.Find("img").GetAttribute("src").ShouldBe("new.png"));
        (await source.GetPartyAvatarUrlAsync(partyId)).ShouldBe("new.png");
    }

    [Fact]
    public async Task AvatarSource_CoalescesPendingRequests_AndRetriesTransportFailure()
    {
        var source = new DeferredPhotoSource();
        var partyId = Guid.NewGuid();
        var first = source.GetPartyAvatarUrlAsync(partyId);
        var second = source.GetPartyAvatarUrlAsync(partyId);
        source.Reads.ShouldBe(1);
        source.Request.SetException(new IOException("Unavailable"));
        await Should.ThrowAsync<IOException>(first);
        await Should.ThrowAsync<IOException>(second);
        source.Request = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var retry = source.GetPartyAvatarUrlAsync(partyId);
        source.Reads.ShouldBe(2);
        source.Request.SetResult(null);
        (await retry).ShouldBeNull();
    }

    private sealed class DeferredPhotoSource : CnAvatarSource
    {
        public TaskCompletionSource<string?> Request { get; set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int Reads { get; private set; }
        protected override Task<string?> LoadPartyAvatarUrlAsync(Guid partyId)
        {
            Reads++;
            return Request.Task;
        }
        public void Save(Guid partyId, string url) => SetPartyAvatarUrl(partyId, url);
    }
}
