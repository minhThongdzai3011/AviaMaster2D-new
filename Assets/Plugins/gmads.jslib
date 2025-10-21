mergeInto(LibraryManager.library, {
    GMInitADS: function(version) {},
    GMRequestAd: function(adType) {
        var callbacks = {
            adStarted: function() {
                SendMessage('GMSDKAdvertisement', 'JSLibCallback_AdStarted');
            },
            adFinished: function() {
                SendMessage('GMSDKAdvertisement', 'JSLibCallback_AdFinished');
            },
            adError: function(error) {
                SendMessage('GMSDKAdvertisement', 'JSLibCallback_AdError', JSON.stringify(error));
            }
        };

        // Chọn quảng cáo ngẫu nhiên
        var ads = window["GMSOFT_GMADS_INFO"].reward;
        var selectedAd = ads[Math.floor(Math.random() * ads.length)];

        try {
            // Tạo overlay container
            var overlay = document.createElement('div');
            overlay.id = 'gm-ad-overlay';
            overlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.9);
        z-index: 9999;
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
      `;

            // Tạo container cho video
            var videoContainer = document.createElement('div');
            videoContainer.style.cssText = `
        position: relative;
        max-width: 90%;
        max-height: 80%;
        background: #000;
        border-radius: 8px;
        overflow: hidden;
      `;

            // Tạo video element
            var video = document.createElement('video');
            video.src = selectedAd.image;
            video.style.cssText = `
        width: 100%;
        height: 100%;
        display: block;
      `;
            video.autoplay = true;
            video.muted = true; // Bắt buộc để autoplay hoạt động
            video.preload = 'auto';

            // Tạo nút skip
            var skipButton = document.createElement('button');
            skipButton.innerHTML = 'Skip (5)';
            skipButton.disabled = true;
            skipButton.style.cssText = `
        position: absolute;
        top: 10px;
        right: 10px;
        background: rgba(0, 0, 0, 0.8);
        color: white;
        border: 1px solid #666;
        border-radius: 6px;
        padding: 10px 16px;
        cursor: pointer;
        font-size: 16px;
        font-weight: bold;
        z-index: 10001;
        box-shadow: 0 2px 8px rgba(0,0,0,0.3);
      `;
            skipButton.disabled = true;
            skipButton.style.opacity = '0.5';

            // Tạo thanh tiến trình
            var progressContainer = document.createElement('div');
            progressContainer.style.cssText = `
        position: absolute;
        bottom: 0;
        left: 0;
        width: 100%;
        height: 6px;
        background: rgba(255, 255, 255, 0.3);
        z-index: 10001;
      `;

            var progressBar = document.createElement('div');
            progressBar.style.cssText = `
        height: 100%;
        width: 0%;
        background: linear-gradient(90deg, #ff6b6b, #4ecdc4);
        transition: width 0.1s ease;
      `;

            // Countdown cho nút skip
            var skipCountdown = 5;
            var skipTimer = setInterval(function() {
                skipCountdown--;
                if (skipCountdown > 0) {
                    skipButton.innerHTML = 'Skip (' + skipCountdown + ')';
                } else {
                    skipButton.innerHTML = 'Skip';
                    skipButton.disabled = false;
                    skipButton.style.opacity = '1';
                    skipButton.style.cursor = 'pointer';
                    clearInterval(skipTimer);
                }
            }, 1000);

            // Hàm đóng quảng cáo
            function closeAd() {
                if (overlay && overlay.parentNode) {
                    overlay.parentNode.removeChild(overlay);
                }
                clearInterval(skipTimer);
                callbacks.adFinished();
            }

            // Hàm mở URL quảng cáo
            function openAdUrl() {
                window.open(selectedAd.url, '_blank');
            }

            // Hàm cập nhật thanh tiến trình
            function updateProgress() {
                if (video.duration && video.currentTime) {
                    var progress = (video.currentTime / video.duration) * 100;
                    progressBar.style.width = progress + '%';
                }
            }

            // Event listeners
            video.addEventListener('loadstart', function() {
                callbacks.adStarted();
            });

            video.addEventListener('timeupdate', function() {
                updateProgress();
            });

            video.addEventListener('ended', function() {
                closeAd();
            });

            video.addEventListener('error', function(e) {
                var error = {
                    message: 'Video load error',
                    code: e.target.error ? e.target.error.code : 'unknown'
                };
                callbacks.adError(error);
                closeAd();
            });

            video.addEventListener('click', function() {
                openAdUrl();
            });

            skipButton.addEventListener('click', function() {
                if (!skipButton.disabled) {
                    closeAd();
                }
            });

            // Ngăn chặn right-click trên video
            video.addEventListener('contextmenu', function(e) {
                e.preventDefault();
            });

            // Thêm elements vào DOM
            progressContainer.appendChild(progressBar);
            videoContainer.appendChild(video);
            videoContainer.appendChild(skipButton);
            videoContainer.appendChild(progressContainer);
            overlay.appendChild(videoContainer);
            document.body.appendChild(overlay);

            // Bắt đầu phát video
            video.play().catch(function(error) {
                console.error('Video play error:', error);
                callbacks.adError({
                    message: 'Cannot play video',
                    details: error.message
                });
                closeAd();
            });

        } catch (error) {
            console.error('Ad overlay creation error:', error);
            callbacks.adError({
                message: 'Failed to create ad overlay',
                details: error.message
            });
        }
    }

});