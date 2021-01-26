//
//  AVProVideoTypes.h
//  AVProVideo
//
//  Created by Morris Butler on 02/10/2020.
//  Copyright © 2020 RenderHeads. All rights reserved.
//

#ifndef AVProVideoTypes_h
#define AVProVideoTypes_h

#import <Foundation/Foundation.h>

typedef void * AVPPlayerRef;

// Video settings

/// Supported video pixel format types.
/// @const AVPPlayerVideoPixelFormatInvalid
/// @const AVPPlayerVideoPixelFormatBgra Generic Planar RGBA pixel format, includes BGRA32, DXTn, etc.
/// @const AVPPlayerVideoPixelFormatYCbCr420 Bi-planar Y'CbCr pixel format.
typedef NS_ENUM(int, AVPPlayerVideoPixelFormat)
{
	AVPPlayerVideoPixelFormatInvalid,
	AVPPlayerVideoPixelFormatBgra,
	AVPPlayerVideoPixelFormatYCbCr420
};

typedef NS_OPTIONS(int, AVPPlayerVideoOutputSettingsFlags)
{
	AVPPlayerVideoOutputSettingsFlagsNone            = 0,
	AVPPlayerVideoOutputSettingsFlagsLinear          = 1 << 0,
	AVPPlayerVideoOutputSettingsFlagsGenerateMipMaps = 1 << 1,
};

typedef struct
{
	float width;
	float height;
} AVPPlayerDimensions;

typedef struct
{
	AVPPlayerVideoPixelFormat pixelFormat;
	AVPPlayerVideoOutputSettingsFlags flags;
	AVPPlayerDimensions preferredMaximumResolution;
} AVPPlayerVideoOutputSettings;

// Audio settings

typedef NS_ENUM(int, AVPPlayerAudioOutputMode)
{
	AVPPlayerAudioOutputModeSystemDirect,
	AVPPlayerAudioOutputModeCapture,
};

typedef NS_OPTIONS(int, AVPPlayerAudioOutputSettingsFlags)
{
	AVPPlayerAudioOutputSettingsFlagsNone            = 0,
};

typedef struct
{
	AVPPlayerAudioOutputMode mode;
	int sampleRate;
	AVPPlayerAudioOutputSettingsFlags flags;
} AVPPlayerAudioOutputSettings;

typedef struct
{
	double preferredPeakBitRate;
} AVPPlayerNetworkSettings;

typedef struct
{
	AVPPlayerVideoOutputSettings videoOutputSettings;
	AVPPlayerAudioOutputSettings audioOutputSettings;
	AVPPlayerNetworkSettings networkSettings;
} AVPPlayerSettings;

// Player state

///
typedef NS_OPTIONS(int, AVPPlayerStatus)
{
	AVPPlayerStatusUnknown                   = 0,
	AVPPlayerStatusReadyToPlay               = 1 <<  0,
	AVPPlayerStatusPlaying                   = 1 <<  1,
	AVPPlayerStatusPaused                    = 1 <<  2,
	AVPPlayerStatusFinished                  = 1 <<  3,
	AVPPlayerStatusSeeking                   = 1 <<  4,
	AVPPlayerStatusBuffering                 = 1 <<  5,
	AVPPlayerStatusStalled                   = 1 <<  6,
	AVPPlayerStatusExternalPlaybackActive    = 1 <<  7,

	AVPPlayerStatusUpdatedAssetInfo          = 1 << 16,
	AVPPlayerStatusUpdatedTexture            = 1 << 17,
	AVPPlayerStatusUpdatedBufferedTimeRanges = 1 << 18,
	AVPPlayerStatusUpdatedSeekableTimeRanges = 1 << 19,
	AVPPlayerStatusUpdatedText               = 1 << 20,
	
	AVPPlayerStatusHasVideo                  = 1 << 24,
	AVPPlayerStatusHasAudio                  = 1 << 25,
	AVPPlayerStatusHasText                   = 1 << 26,
	AVPPlayerStatusHasMetadata               = 1 << 27,
	
	AVPPlayerStatusFailed                    = 1 << 31
};

typedef NS_OPTIONS(int, AVPPlayerFlags)
{
	AVPPlayerFlagsNone                  = 0,
	AVPPlayerFlagsLooping               = 1 <<  0,
	AVPPlayerFlagsMuted                 = 1 <<  1,
	AVPPlayerFlagsAllowExternalPlayback = 1 <<  2,
	AVPPlayerFlagsDirty                 = 1 << 31,
};

typedef NS_ENUM(int, AVPPlayerExternalPlaybackVideoGravity)
{
	AVPPlayerExternalPlaybackVideoGravityResize,
	AVPPlayerExternalPlaybackVideoGravityResizeAspect,
	AVPPlayerExternalPlaybackVideoGravityResizeAspectFill
};

typedef struct
{
	AVPPlayerStatus status;
	double currentTime;
	double currentDate;
	int selectedVideoTrack;
	int selectedAudioTrack;
	int selectedTextTrack;
	int bufferedTimeRangesCount;
	int seekableTimeRangesCount;
} AVPPlayerState;

typedef NS_OPTIONS(int, AVPPlayerAssetFlags)
{
	AVPPlayerAssetFlagsNone = 0,
	AVPPlayerAssetFlagsCompatibleWithAirPlay = 1 << 0,
};

typedef struct
{
	double duration;
	AVPPlayerDimensions dimensions;
	float frameRate;
	int videoTrackCount;
	int audioTrackCount;
	int textTrackCount;
	AVPPlayerAssetFlags flags;
} AVPPlayerAssetInfo;

typedef NS_OPTIONS(int, AVPPlayerTrackFlags)
{
	AVPPlayerTrackFlagsNone    = 0,
	AVPPlayerTrackFlagsDefault = 1 << 0,
};

typedef NS_ENUM(int, AVPPlayerVideoTrackStereoMode)
{
	AVPPlayerVideoTrackStereoModeUnknown,
	AVPPlayerVideoTrackStereoModeMonoscopic,
	AVPPlayerVideoTrackStereoModeStereoscopicTopBottom,
	AVPPlayerVideoTrackStereoModeStereoscopicLeftRight,
	AVPPlayerVideoTrackStereoModeStereoscopicCustom,
	AVPPlayerVideoTrackStereoModeStereoscopicRightLeft,
};

typedef struct
{
	float a;
	float b;
	float c;
	float d;
	float tx;
	float ty;
} AVPAffineTransform;

typedef struct
{
	unichar * _Nullable name;
	unichar * _Nullable language;
	int trackID;
	float estimatedDataRate;
	uint32_t codecSubtype;
	AVPPlayerTrackFlags flags;

	AVPPlayerDimensions dimensions;
	float frameRate;
	AVPAffineTransform transform;
	AVPPlayerVideoTrackStereoMode stereoMode;

} AVPPlayerVideoTrackInfo;

/// Audio channel bitmap
/// @const AVPPlayerAudioTrackChannelBitmapUnspecified No channels specified.
typedef NS_OPTIONS(uint32_t, AVPPlayerAudioTrackChannelBitmap)
{
	AVPPlayerAudioTrackChannelBitmapUnspecified 		= 0,
	AVPPlayerAudioTrackChannelBitmapFrontLeft 			= 1 <<  0,
	AVPPlayerAudioTrackChannelBitmapFrontRight 			= 1 <<  1,
	AVPPlayerAudioTrackChannelBitmapFrontCenter 		= 1 <<  2,
	AVPPlayerAudioTrackChannelBitmapLowFrequency 		= 1 <<  3,
	AVPPlayerAudioTrackChannelBitmapBackLeft 			= 1 <<  4,
	AVPPlayerAudioTrackChannelBitmapBackRight 			= 1 <<  5,
	AVPPlayerAudioTrackChannelBitmapFrontLeftOfCenter 	= 1 <<  6,
	AVPPlayerAudioTrackChannelBitmapFrontRightOfCenter 	= 1 <<  7,
	AVPPlayerAudioTrackChannelBitmapBackCenter 			= 1 <<  8,
	AVPPlayerAudioTrackChannelBitmapSideLeft 			= 1 <<  9,
	AVPPlayerAudioTrackChannelBitmapSideRight 			= 1 << 10,
	AVPPlayerAudioTrackChannelBitmapTopCenter 			= 1 << 11,
	AVPPlayerAudioTrackChannelBitmapTopFrontLeft 		= 1 << 12,
	AVPPlayerAudioTrackChannelBitmapTopFrontCenter 		= 1 << 13,
	AVPPlayerAudioTrackChannelBitmapTopFrontRight 		= 1 << 14,
	AVPPlayerAudioTrackChannelBitmapTopBackLeft 		= 1 << 15,
	AVPPlayerAudioTrackChannelBitmapTopBackCenter 		= 1 << 16,
	AVPPlayerAudioTrackChannelBitmapTopBackRight 		= 1 << 17,
};


typedef struct
{
	unichar * _Nullable name;
	unichar * _Nullable language;
	int trackID;
	float estimatedDataRate;
	uint32_t codecSubtype;
	AVPPlayerTrackFlags flags;
	
	double sampleRate;
	uint32_t channelCount;
	uint32_t channelLayoutTag;
	AVPPlayerAudioTrackChannelBitmap channelBitmap;

} AVPPlayerAudioTrackInfo;

typedef struct
{
	unichar * _Nullable name;
	unichar * _Nullable language;
	int trackID;
	float estimatedDataRate;
	uint32_t codecSubtype;
	AVPPlayerTrackFlags flags;

} AVPPlayerTextTrackInfo;

typedef NS_ENUM(int, AVPPlayerTrackType)
{
	AVPPlayerTrackTypeVideo,
	AVPPlayerTrackTypeAudio,
	AVPPlayerTrackTypeText
};

typedef struct
{
	double start;
	double duration;
} AVPPlayerTimeRange;

/// Texture flags.
/// @const AVPPlayerTextureFlagsFlipped The texture is flipped in the y-axis.
/// @const AVPlayerTextureFlagsLinear The texture uses the linear color space.
/// @const AVPPlayerTextureFlagsMipmapped The texture has mipmaps.
typedef NS_OPTIONS(int, AVPPlayerTextureFlags)
{
	AVPPlayerTextureFlagsNone      = 0,
	AVPPlayerTextureFlagsFlipped   = 1 << 0,
	AVPPlayerTextureFlagsLinear    = 1 << 1,
	AVPPlayerTextureFlagsMipmapped = 1 << 2,
};

typedef NS_ENUM(int, AVPPlayerTextureYCbCrMatrix)
{
	AVPPlayerTextureYCbCrMatrixIdentity,
	AVPPlayerTextureYCbCrMatrixITU_R_601,
	AVPPlayerTextureYCbCrMatrixITU_R_709,
};


typedef NS_ENUM(int, AVPPlayerTextureFormat)
{
	AVPPlayerTextureFormatInvalid,
	AVPPlayerTextureFormatBgra8Unorm,
	AVPPlayerTextureFormatR8Unorm,
	AVPPlayerTextureFormatRg8Unorm,
	AVPPlayerTextureFormatBc1,
	AVPPlayerTextureFormatBc3,
	AVPPlayerTextureFormatBc4,
	AVPPlayerTextureFormatBc5,
	AVPPlayerTextureFormatBc7,
};

typedef struct
{
	void * _Nullable plane;
	int width;
	int height;
	AVPPlayerTextureFormat pixelFormat;
} AVPPlayerTexturePlane;

#define AVPPlayerTextureMaxPlanes 2

typedef struct
{
	AVPPlayerTexturePlane planes[AVPPlayerTextureMaxPlanes];
	int64_t itemTime;
	int frameCount;
	int planeCount;
	AVPPlayerTextureFlags flags;
	AVPPlayerTextureYCbCrMatrix YCbCrMatrix;
} AVPPlayerTexture;

typedef struct
{
	void * _Nullable buffer;
	int64_t itemTime;
	int32_t length;
	int32_t sequence;
} AVPPlayerText;

typedef struct AudioCaptureBuffer *AudioCaptureBufferRef;

// MARK: Logging

/// @enum Log flags
/// @discussion
/// @const AVPLogFlagError
/// @const AVPLogFlagWarning
/// @const AVPLogFlagInfo
/// @const AVPLogFlagDebug
/// @const AVPLogFlagVerbose
typedef NS_OPTIONS(int, AVPLogFlag)
{
	AVPLogFlagError   = 1 << 0,
	AVPLogFlagWarning = 1 << 1,
	AVPLogFlagInfo    = 1 << 2,
	AVPLogFlagDebug   = 1 << 3,
	AVPLogFlagVerbose = 1 << 4,
};

/// @enum Log levels
/// @discussion
/// @const AVPLogLevelOff
/// @const AVPLogLevelError
/// @const AVPLogLevelWarning
/// @const AVPLogLevelInfo
/// @const AVPLogLevelDebug
/// @const AVPLogLevelVerbose
typedef NS_ENUM(int, AVPLogLevel)
{
	AVPLogLevelOff     = 0,
	AVPLogLevelError   = AVPLogFlagError,
	AVPLogLevelWarning = AVPLogFlagWarning | AVPLogLevelError,
	AVPLogLevelInfo    = AVPLogFlagInfo    | AVPLogLevelWarning,
	AVPLogLevelDebug   = AVPLogFlagDebug   | AVPLogLevelInfo,
	AVPLogLevelVerbose = AVPLogFlagVerbose | AVPLogLevelDebug,
};

/// @typedef AVPLogCallback
/// @abstract Log callback type.
/// @discussion Log callback for passing messages from the Logging system back via a C callback function.
/// @param level The log level for the message.
/// @prarm message The message to log.
typedef void (*AVPLogCallback)(AVPLogLevel level, const void * _Nonnull message);

#endif /* AVProVideoTypes_h */
