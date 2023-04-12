#import "UMSPInterstitialAdShowDelegate.h"

@implementation UMSPInterstitialAdShowDelegate

- (id)initWithStartedCallback:(StartedCallback)started clickedCallback:(ClickedCallback)clicked finishedCallback:(FinishedCallback)finished failedShowCallback:(FailedShowCallback)failedShow {
    self = [super init];

    if (self) {
        self.started = started;
        self.clicked = clicked;
        self.finished = finished;
        self.failedShow = failedShow;
    }

    return self;
}

- (void)onInterstitialClicked:(UMSInterstitialAd *)interstitialAd {
    dispatch_async(dispatch_get_main_queue(), ^{
        if (self.clicked) {
            self.clicked((__bridge void *)interstitialAd);
        }
    });
}

- (void)onInterstitialClosed:(UMSInterstitialAd *)interstitialAd {
    dispatch_async(dispatch_get_main_queue(), ^{
        if (self.finished) {
            self.finished((__bridge void *)interstitialAd);
        }
    });
}

- (void)onInterstitialFailedShow:(UMSInterstitialAd *)interstitialAd error:(UMSShowError)error message:(NSString *)message {
    dispatch_async(dispatch_get_main_queue(), ^{
        if (self.failedShow) {
            self.failedShow((__bridge void *)interstitialAd, (int)error, [message UTF8String]);
        }
    });
}

- (void)onInterstitialImpression:(UMSInterstitialAd *)interstitialAd {
}

- (void)onInterstitialShowed:(UMSInterstitialAd *)interstitialAd {
    dispatch_async(dispatch_get_main_queue(), ^{
        if (self.started) {
            self.started((__bridge void *)interstitialAd);
        }
    });
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void * UMSPInterstitialAdShowDelegateCreate(StartedCallback startedCallback, ClickedCallback clickedCallback, FinishedCallback finishedCallback, FailedShowCallback failedShowCallback) {
    UMSPInterstitialAdShowDelegate *delegate = [[UMSPInterstitialAdShowDelegate alloc] initWithStartedCallback:startedCallback clickedCallback:clickedCallback finishedCallback:finishedCallback failedShowCallback:failedShowCallback];

    return (__bridge_retained void *)delegate;
}

void UMSPInterstitialAdShowDelegateDestroy(void *ptr) {
    if (!ptr) return;

    UMSPInterstitialAdShowDelegate *delegate = (__bridge_transfer UMSPInterstitialAdShowDelegate *)ptr;

    delegate.started = nil;
    delegate.clicked = nil;
    delegate.finished = nil;
    delegate.failedShow = nil;
}

#ifdef __cplusplus
}
#endif
