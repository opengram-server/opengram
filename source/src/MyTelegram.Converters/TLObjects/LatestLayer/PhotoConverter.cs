namespace MyTelegram.Converters.TLObjects.LatestLayer;

public class PhotoConverter : IPhotoConverter, ITransientDependency
{
    
    public virtual int Layer => Layers.LayerLatest;

    public virtual IChatPhoto ToChatPhoto(IPhotoReadModel? photoReadModel)
    {
        if (photoReadModel == null)
        {
            return new TChatPhotoEmpty();
        }

        return new TChatPhoto
        {
            DcId = photoReadModel.DcId,
            PhotoId = photoReadModel.PhotoId,
            HasVideo = photoReadModel.VideoSizes2?.Count > 0
        };
    }

    public virtual IPhoto ToPhoto(IPhotoReadModel? photoReadModel)
    {
        if (photoReadModel == null)
        {
            return new TPhotoEmpty();
        }

        var photo = new TPhoto
        {
            HasStickers = photoReadModel.HasStickers,
            Id = photoReadModel.PhotoId,
            AccessHash = photoReadModel.AccessHash,
            Date = photoReadModel.Date,
            DcId = photoReadModel.DcId,
            FileReference = photoReadModel.FileReference
        };

        if (photoReadModel.Sizes2 != null)
        {
            photo.Sizes = new TVector<IPhotoSize>(photoReadModel.Sizes2);
        }

        if (photoReadModel.VideoSizes2 != null)
        {
            photo.VideoSizes = new TVector<IVideoSize>(photoReadModel.VideoSizes2);
        }

        // Used for compatibility with old data, new data will only use Sizes2 and VideoSizes2
        if (photoReadModel.Sizes?.Count > 0)
        {
            photo.Sizes = new TVector<IPhotoSize>();
            foreach (var s in photoReadModel.Sizes)
            {
                //photo.Sizes.Add(new TPhotoSize
                //{
                //    H = s.H,
                //    W = s.W,
                //    Size = (int)s.Size,
                //    Type = s.Type
                //});
                IPhotoSize size;
                switch (s.Type)
                {
                    case "i":
                        size = new TPhotoStrippedSize
                        {
                            Bytes = s.StrippedThumb,
                            Type = s.Type
                        };
                        break;
                    default:
                        size = new TPhotoSize
                        {
                            H = s.H,
                            W = s.W,
                            Size = (int)s.Size,
                            Type = s.Type
                        };
                        break;
                }

                photo.Sizes.Add(size);
            }
        }

        if (photoReadModel.VideoSizes?.Count > 0)
        {
            photo.VideoSizes = new TVector<IVideoSize>();
            foreach (var s in photoReadModel.VideoSizes)
            {
                photo.VideoSizes.Add(new TVideoSize
                {
                    H = s.H,
                    W = s.W,
                    Size = (int)s.Size,
                    Type = s.Type,
                    VideoStartTs = s.VideoStartTs
                });
            }
        }

        return photo;
    }

    public virtual IUserProfilePhoto ToProfilePhoto(IPhotoReadModel? photoReadModel)
    {
        if (photoReadModel == null)
        {
            return new TUserProfilePhotoEmpty();
        }

        var strippedThumbSize = photoReadModel.Sizes?.FirstOrDefault(p => p.StrippedThumb?.Length > 0);

        return new TUserProfilePhoto
        {
            DcId = photoReadModel.DcId,
            PhotoId = photoReadModel.PhotoId,
            HasVideo = photoReadModel.VideoSizes2?.Count > 0,
            StrippedThumb = strippedThumbSize?.StrippedThumb
            //Personal = 
            //StrippedThumb = 
        };
    }
}