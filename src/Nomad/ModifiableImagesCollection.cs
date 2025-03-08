using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Nomad;
using OwlCore.Nomad.Kubo;
using OwlCore.Nomad.Kubo.Events;
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
        
        var updateEvent = new ValueUpdateEvent((DagCid)keyCid, (DagCid)valueCid, false);
        
        var appendedEntry = await AppendNewEntryAsync(targetId: Id, eventId: nameof(AddImageAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, newImage, cancellationToken);

        EventStreamPosition = appendedEntry;
    }

    /// <inheritdoc />
    public async Task RemoveImageAsync(IFile imageFile, CancellationToken cancellationToken)
    {
        var image = Inner.Inner.Images.FirstOrDefault(img => img.Id == imageFile.Id);
        if (image == null)
        {
            throw new ArgumentException("Image not found in the collection.", nameof(imageFile));
        }

        var keyCid = await Client.Dag.PutAsync(image.Id, pin: KuboOptions.ShouldPin, cancel: cancellationToken);
        var valueCid = await Client.Dag.PutAsync(image, pin: KuboOptions.ShouldPin, cancel: cancellationToken);

        var updateEvent = new ValueUpdateEvent((DagCid)keyCid, (DagCid)valueCid, true);

        var appendedEntry = await AppendNewEntryAsync(Id, nameof(RemoveImageAsync), updateEvent, DateTime.UtcNow, cancellationToken);
        await ApplyEntryUpdateAsync(appendedEntry, updateEvent, image, cancellationToken);

        EventStreamPosition = appendedEntry;
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
    /// If already have a resolved instance of <see cref="Image"/>, you should call <see cref="ApplyEntryUpdateAsync(EventStreamEntry{DagCid}, ValueUpdateEvent, Image, CancellationToken)"/> instead.
    /// </remarks>
    /// <param name="eventStreamEntry">The event stream entry to apply.</param>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public override async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (eventStreamEntry.TargetId != Id)
            return;
        
        Guard.IsNotNull(updateEvent.Value);
        var (image, _) = await Client.ResolveDagCidAsync<Image>(updateEvent.Value.Value, nocache: !KuboOptions.UseCache, cancellationToken);
        
        Guard.IsNotNull(image);
        await ApplyEntryUpdateAsync(eventStreamEntry, updateEvent, image, cancellationToken);
    }

    /// <summary>
    /// Applies an event stream update event and raises the relevant events.
    /// </summary>
    /// <param name="eventStreamEntry">The event stream entry to apply.</param>
    /// <param name="updateEvent">The update event to apply.</param>
    /// <param name="image">The resolved image data for this event.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task ApplyEntryUpdateAsync(EventStreamEntry<DagCid> eventStreamEntry, ValueUpdateEvent updateEvent, Image image, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (eventStreamEntry.EventId)
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

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        EventStreamPosition = null;
        Inner.Inner.Images = [];
            
        return Task.CompletedTask;
    }
}
