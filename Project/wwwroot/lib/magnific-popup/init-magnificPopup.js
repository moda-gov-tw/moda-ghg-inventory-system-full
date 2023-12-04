/*
//使用說明
//example 1: subinitA('800px', '600px', '../SubWin/EditExcelImport.aspx')
//example 2: subinitA_func('800px', '600px', '../SubWin/EditExcelImport.aspx','') 最後一個參數，如沒有callbackfunction，帶入空值即可
//style.css 117行 width:95%; ，子視窗 div大小為 800px*0.95=760px，使scrollbar貼近iframe邊框
*/
function subinitA(subwidth, subheight, suburl) {
    //console.log('Open SubWin');
    $.magnificPopup.open({
        type: 'iframe',
        midClick: false, // 是否使用滑鼠中鍵
        closeOnBgClick: false,//點擊背景關閉視窗
        showCloseBtn: false,//隱藏關閉按鈕
        fixedContentPos: true,//彈出視窗是否固定在畫面上
        mainClass: 'mfp-fade',//加入CSS淡入淡出效果
        tClose: '關閉',//翻譯字串
        iframe: {
            //markup: 
            //'<div class="mfp-iframe-scalerA">' +
            //'<div class="mfp-close"></div>' +
            //'<iframe class="mfp-iframe" frameborder="0" allowfullscreen scrolling="no"></iframe>' +
            //'</div>',

            markup:
                '<div class="mfp-iframe-scalerA">' +
                '<div class="mfp-close"></div>' +
                '<iframe class="mfp-iframe" frameborder="0" allowfullscreen scrolling="no" style="width:' + subwidth + '; height:' + subheight + ';" ></iframe>' +
                '</div>',


            //markup: '<div style="width:' + subwidth + '; height:' + subheight + ';">' + 
            //'<div class="mfp-iframe-scalerA">' +
            //'<div class="mfp-close"></div>' +
            //    '<iframe class="mfp-iframe" frameborder="0" allowfullscreen scrolling="no" style="width:' + subwidth + '; height:' + subheight + ';" ></iframe>' +
            //'</div></div>',
        },
        items: {
            src: suburl
        },
        callbacks: {
            close: function () {
                //console.log('Close SubWin');

            }
        },
    });
}

function subinitB(subwidth, subheight, suburl, title) {

    //console.log('Open SubWin');
    $.magnificPopup.open({
        type: 'iframe',
        midClick: false,
        closeOnBgClick: false,
        showCloseBtn: false,
        mainClass: 'mfp-fade',
        tClose: '關閉',
        iframe: {
            markup:
                '<div class="magpopupiframeTitle" style="width:' + subwidth + '">' + title + '</div>' +
                '<div class=" iframe-container">' +
                '<iframe class="mfp-iframe" frameborder="0" style="width:' + subwidth + '" allowfullscreen scrolling="yes"></iframe>' +
                '</div>',
            draggable: true // 添加这行以启用可拖动功能
        },
        items: {
            src: suburl
        },
        callbacks: {
            open: function () {
                var iframe = this.content.find('iframe');
                iframe.attr('src', suburl);
            },
            close: function () {
                var iframe = this.content.find('iframe');
                iframe.attr('src', ''); // 清除 iframe 的 src 属性
                //console.log('Close SubWin');
            }
        }
    });
}

function subinitA_func(subwidth, subheight, suburl, refreshCallbackFunc) {
    $.magnificPopup.open({
        type: 'iframe',
        midClick: false, // 是否使用滑鼠中鍵
        closeOnBgClick: false,//點擊背景關閉視窗
        showCloseBtn: true,//隱藏關閉按鈕
        fixedContentPos: true,//彈出視窗是否固定在畫面上
        mainClass: 'mfp-fade',//加入CSS淡入淡出效果
        tClose: '關閉',//翻譯字串
        iframe: {
            markup: '<div style="width:' + subwidth + '; height:' + subheight + ';">' +
                '<div class="mfp-iframe-scalerA">' +
                '<div class="mfp-close"></div>' +
                '<iframe class="mfp-iframe" frameborder="0" allowfullscreen scrolling="no"></iframe>' +
                '</div></div>',
        },
        items: {
            src: suburl
        },
        callbacks: {
            close: refreshCallbackFunc
        },
    });
}
