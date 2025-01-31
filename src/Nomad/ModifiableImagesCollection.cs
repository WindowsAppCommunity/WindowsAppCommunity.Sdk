using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Storage;
using WindowsAppCommunity.Sdk.Models;

namespace WindowsAppCommunity.Sdk.Nomad;

/// <inheritdoc cref="IModifiableImagesCollection" />
public class ModifiableImagesCollection : NomadKuboEventStreamHandler<ValueUpdateEvent>, IModifiableImagesCollection, IDelegable<ReadOnlyImagesCollection>
{
    /// <inheritdoc />
    public required ReadOnlyImagesCollection Inner { get; init; }
    
    /// <summary>
    /// A unique identifier for this instance, persistent across machines and reruns.
    /// </summary>
    public required string Id { get; init; }
    
    /// <inheritdoc />
    public async Task AddImageAsync(IFile imageFile, CancellationToken cancellationToken)
    {
        var imageCid = await imageFile.GetCidAsync(Inner.Client, new AddFileOptions { Pin = KuboOptions.ShouldPin, }, cancellationToken);
        
        var newImage = new Image
        {
            Id = imageFile.Id,
            Name = imageFile.Name,
            Cid = (DagCid)imageCid,
        };
        
        var keyCid = await Client.Dag.PutAsync(newImage.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(newImage, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        
        var updateEvent = new ValueUpdateEvent(Id, nameof(AddImageAsync), (DagCid)keyCid, (DagCid)valueCid, false);
        
        await ApplyEntryUpdateAsync(updateEvent, newImage, cancellationToken);
        var appendedEntry = await AppendNewEntryAsync(updateEvent, cancellationToken);

        EventStreamPosition = appendedEntry;

        // Append entry to event stream
        // TODO
    }

    /// <inheritdoc />
    public Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken) => Inner.GetImageFilesAsync(cancellationToken);

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesAdded;

    /// <inheritdoc />
    public event EventHandler<IFile[]>? ImagesRemoved;

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <remarks>
    /// This method will call <see cref="ReadOnlyImagesCollection.GetAsync(string, CancellationToken)"/> and create a new instance to pass to the event handlers.
    /// <para/>
    /// If already have a resolved instance of <see cref="Image"/>, you should call <see cref="ApplyEntryUpdateAsync(ValueUpdateEvent, Image, CancellationToken)"/> instead.
    /// </remarks>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public override async Task ApplyEntryUpdateAsync(ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (updateEvent.TargetId != Id)
            return;
        
        Guard.IsNotNull(updateEvent.Value);
        var (image, _) = await Client.ResolveDagCidAsync<Image>(updateEvent.Value.Value, nocache: !KuboOptions.UseCache, cancellationToken);
        
        Guard.IsNotNull(image);
        await ApplyEntryUpdateAsync(updateEvent, image, cancellationToken);
    }

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="image">The resolved image data for this event.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task ApplyEntryUpdateAsync(ValueUpdateEvent updateEvent, Image image, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (updateEvent.EventId)
        {
            case nameof(AddImageAsync):
            {
                var imageFile = await Inner.GetAsync(image.Id, cancellationToken);
                Inner.Inner.Images = [..Inner.Inner.Images, image];
                ImagesAdded?.Invoke(this, [imageFile]); 
                break;
            }
            case nameof(RemoveImageAsync):
            {
                var imageFile = await Inner.GetAsync(image.Id, cancellationToken);
                Inner.Inner.Images = [.. Inner.Inner.Images.Except([image])];
                ImagesRemoved?.Invoke(this, [imageFile]);
                break;
            }
        }
    }

    /// <inheritdoc cref="INomadKuboEventStreamHandler{TEventEntryContent}.AppendNewEntryAsync" />
    public override async Task<EventStreamEntry<Cid>> AppendNewEntryAsync(ValueUpdateEvent updateEvent, CancellationToken cancellationToken = default)
    {
        // Use extension method for code deduplication (can't use inheritance).
        var localUpdateEventCid = await Client.Dag.PutAsync(updateEvent, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var newEntry = await this.AppendEventStreamEntryAsync(localUpdateEventCid, updateEvent.EventId, updateEvent.TargetId, cancellationToken);
        return newEntry;
    }

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        EventStreamPosition = null;
        Inner.Inner.Images = [];
            
        return Task.CompletedTask;
    }
}