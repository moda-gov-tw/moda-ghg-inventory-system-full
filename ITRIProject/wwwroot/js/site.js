// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showTooltip(event) {
    const tooltip = event.target.querySelector('.tooltip-text');
    tooltip.style.left = `${event.clientX}px`;
}

//計算當前月份天數
function getDaysInMonth(month, year) {

    let currentYear = parseInt(year) + 1911;
    //第一個欄位如果是2月開始，則判斷是否為閏年
    if (month == 2) {
        let isLeap = isLeapYear(currentYear);
        let daysInFebruary = isLeap ? 29 : 28;
        return daysInFebruary;
    }
    let daysInDecember = new Date(currentYear, month, 0).getDate();

    return daysInDecember;
}
//判斷是否為閏年

function isLeapYear(year) {
    return (year % 4 === 0 && year % 100 !== 0) || year % 400 === 0;
}

//日期輸入限制
function validateDate(inputElement) {
    const datePattern = /^(\d{3}\/)?([1-9]|1[0-2])\/([1-9]|[12][0-9]|3[01])$/;

    const inputValue = inputElement.value.trim();

    if (inputValue === '') {
        // 空值，验证通过
        inputElement.classList.remove('dayerror');
        return true;
    }

    if (datePattern.test(inputValue)) {
        const [yearPrefix, month, day] = inputValue.split('/').map(Number);
        const year = yearPrefix ? yearPrefix + 1911 : new Date().getFullYear(); // 将民国年份转换为公历年份

        if (month <= 12) {
            const maxDays = new Date(year, month, 0).getDate();

            if (month === 2 && isLeapYear(year)) {
                // 闰年的二月
                if (day <= 29) {
                    inputElement.classList.remove('dayerror');
                } else {
                    inputElement.classList.add('dayerror');
                }
            } else if (day <= maxDays) {
                // 输入格式和范围都是有效的
                inputElement.classList.remove('dayerror');
            } else {
                // 超过了当前月份的天数限制
                inputElement.classList.add('dayerror');
            }
        } else {
            // 月份超过了有效范围
            inputElement.classList.add('dayerror');
        }
    } else {
        // 输入格式无效
        inputElement.classList.add('dayerror');
    }

    return false; // 表单验证不通过
}

//特殊符號檢查
function validateInput(input) {
    var forbiddenCharacters = /[<>:"\/\\|?*+.]/g;
    var inputValue = input.value;

    if (forbiddenCharacters.test(inputValue)) {
        // 如果輸入包含禁止的字符，則清除該字符
        alert("請勿輸入以下特殊符號: < > : \" / \\ | ? * + .");
        inputValue = inputValue.replace(forbiddenCharacters, '');
        input.value = inputValue;
    }
}

//itme縮放
$(".toggleItem").on("click", function () {
    var content = this.nextElementSibling;
    content.classList.toggle('show');
});

//佐證資料管理，右邊td使用
$(".showContent").on("click", function () {

    event.preventDefault();
    var contentTd = document.getElementById('content-td');
    contentTd.innerHTML = `<iframe src="${this.getAttribute('href') }" style="width: 100%; height: 100%; border: none;"></iframe>`;
});
function ConpanyName() {
    let BusineseNumber = document.getElementById("OrganNumber").value;
    $.ajax({
        url: "/Common/Company",
        type: "POST",
        data: { BusineseNumber: BusineseNumber }, // 手动将数据序列化为JSON字符串
        dataType: "json",
        success: function (response) {
            console.log(response[0].Company_Name);
            const resultLabel = document.getElementById("OrganName");
            resultLabel.value = response[0].Company_Name;
        },
    });
}
$(".printChart-btn").on("click", function () {
    printChart();
});
function printChart() {
    var url = new URL(window.location.href);
    var id = url.pathname.split('/').pop(); // 從 URL 的路徑部分中獲取最後一個值作為 id
    var tableContainer = document.getElementById("tableContainer");
    var layout = document.getElementById("layout");
    var btn = document.getElementById("btn");

    layout.style.display = "none";
    btn.style.display = "none";

    // 打印表格容器中的内容
    window.print();
    window.location.href = "/result/chart/" + id;

}

var checkMonthAdd1 = 0;
function MonthAdd1(OriginalMonth, AddMonth) {
    var inputs = document.getElementsByName('DateTimes');
    console.log(checkMonthAdd1);
    console.log(checkMonthSubtraction1)

    if (OriginalMonth + 1 == 12) {
        OriginalMonth = 1;
    }
    if (checkMonthAdd1 - checkMonthSubtraction1 + AddMonth ==1) {
        alert("月份只能加1次");
        return;
    }
    //if (OriginalMonth < inputs[1].value.split('/')[1].trim()) {
    //    alert("月份只能加1次");
    //    return;
    //}
    for (var i = 0; i < inputs.length; i++) {
        var currentValue = inputs[i].value;
        var parts = currentValue.split('/');
        var year = parseInt(parts[0].trim());
        var month = parseInt(parts[1].trim());
        console.log("month" + month)
        if (month +1> 12) {
            month = 1;
            year++;
        } else {
            month++;
        }
        inputs[i].value = year + " / " + month;
    }
    checkMonthAdd1++;
    $('#AddMonth').val(checkMonthAdd1 - checkMonthSubtraction1 + AddMonth);
}

var checkMonthSubtraction1=0;
function MonthSubtraction1(OriginalMonth, AddMonth) {
    var inputs = document.getElementsByName('DateTimes');

    if (OriginalMonth == 0) {
        OriginalMonth = 12;
    }
    console.log(checkMonthSubtraction1);
    console.log(checkMonthAdd1);
    console.log(AddMonth);

    if (checkMonthSubtraction1 - checkMonthAdd1 - AddMonth ==1) {
        alert("月份只能減1次");
        return;
    }
    //if (OriginalMonth > inputs[1].value.split('/')[1].trim()) {
    //    alert("月份只能減1次");
    //    return;
    //}
    for (var i = 0; i < inputs.length; i++) {
        var currentValue = inputs[i].value;
        var parts = currentValue.split('/');
        var year = parseInt(parts[0].trim());
        var month = parseInt(parts[1].trim());

        if (month -1<=0) {
            month = 12;
            year--;
        } else {
            month--;
        }
        inputs[i].value = year + " / " + month;
    }
    checkMonthSubtraction1++;
    $('#AddMonth').val(checkMonthAdd1 - checkMonthSubtraction1 + AddMonth);

}

function TwoMonthAuto() {
    var startTimeInputs = document.getElementsByName("StartTime");

    startTimeInputs[0].addEventListener('keyup', function () { // 输入时触发
        var firstValue = startTimeInputs[0].value; // 第一筆startTime
        var [year, month, day] = firstValue.split('/').map(Number);//區分年月日

        for (var i = 1; i < startTimeInputs.length; i++) {
            month += 2; // 月份加1
            if (month > 12) {//月份大於12月時，月份從1開始，year加1
                month -= 12;
                year++;
            }

            if (getDaysInMonth(month, year) < day) {//如果自動產出的day大於該月天數，則值以該月天數
                var nextValue = year + '/' + month + '/' + getDaysInMonth(month, year);
            }
            else {
                var nextValue = year + '/' + month + '/' + day;
            }
            startTimeInputs[i].value = nextValue;
        }
    });

    // EndTime自動月份加1帶值
    var endTimeInputs = document.getElementsByName("EndTime");
    endTimeInputs[0].addEventListener('keyup', function () {
        var firstValue = endTimeInputs[0].value;//第一筆endTime
        var [year, month, day] = firstValue.split('/').map(Number);//區分年月日

        for (var i = 1; i < endTimeInputs.length; i++) {
            month += 2; // 月份加1
            if (month > 12) {//月份大於12月時，月份從1開始，year加1
                month -= 12;
                year++;
            }
            if (getDaysInMonth(month, year) < day) {//如果自動產出的day大於該月天數，則值以該月天數
                var nextValue = year + '/' + month + '/' + getDaysInMonth(month, year);
            }
            else {
                var nextValue = year + '/' + month + '/' + day;
            }
            endTimeInputs[i].value = nextValue;
        }
    });
}
function OneMonthAuto() {
    var startTimeInputs = document.getElementsByName("StartTime");

    startTimeInputs[0].addEventListener('keyup', function () { // 输入时触发
        var firstValue = startTimeInputs[0].value; // 第一筆startTime
        var [year, month, day] = firstValue.split('/').map(Number);//區分年月日

        for (var i = 1; i < startTimeInputs.length; i++) {
            month++; // 月份加1
            if (month > 12) {//月份大於12月時，月份從1開始，year加1
                month = 1;
                year++;
            }

            if (getDaysInMonth(month, year) < day) {//如果自動產出的day大於該月天數，則值以該月天數
                var nextValue = year + '/' + month + '/' + getDaysInMonth(month, year);
            }
            else {
                var nextValue = year + '/' + month + '/' + day;
            }
            startTimeInputs[i].value = nextValue;
        }
    });

    // EndTime自動月份加1帶值
    var endTimeInputs = document.getElementsByName("EndTime");
    endTimeInputs[0].addEventListener('keyup', function () {
        var firstValue = endTimeInputs[0].value;//第一筆endTime
        var [year, month, day] = firstValue.split('/').map(Number);//區分年月日

        for (var i = 1; i < endTimeInputs.length; i++) {
            month++; // 月份加1
            if (month > 12) {//月份大於12月時，月份從1開始，year加1
                month = 1;
                year++;
            }
            if (getDaysInMonth(month, year) < day) {//如果自動產出的day大於該月天數，則值以該月天數
                var nextValue = year + '/' + month + '/' + getDaysInMonth(month, year);
            }
            else {
                var nextValue = year + '/' + month + '/' + day;
            }
            endTimeInputs[i].value = nextValue;
        }
    });
}

// 調整字體大小
// 獲取按鈕元素和table1
const smallFontButton = document.getElementById('smallFont');
const mediumFontButton = document.getElementById('mediumFont');
const largeFontButton = document.getElementById('largeFont');
const table1 = document.getElementById('table1');

// 在頁面加載時檢查本地存儲中是否有字體大小設置
const savedFontSize = localStorage.getItem('fontSize');
const savedFontSizeActiveBtn = localStorage.getItem('fontSizeaAtiveBtn');
if (savedFontSize) {
    setFontSize(savedFontSize);
    setButtonActive(savedFontSizeActiveBtn);
}

// 監聽按鈕點擊事件，切換字體大小
smallFontButton.addEventListener('click', function () {
    setFontSize('small');
    saveFontSize('small');
    setButtonActive('smallFont');
    saveFontSizeButtonActive('smallFont');
});

mediumFontButton.addEventListener('click', function () {
    setFontSize('medium');
    saveFontSize('medium');
    setButtonActive('mediumFont');
    saveFontSizeButtonActive('mediumFont');
});

largeFontButton.addEventListener('click', function () {
    setFontSize('large');
    saveFontSize('large');
    setButtonActive('largeFont');
    saveFontSizeButtonActive('largeFont');
});

// 同時更改body和table1的字體大小
function setFontSize(fontSize) {
    document.body.style.fontSize = fontSize;
    if (table1) {
        table1.style.fontSize = fontSize;
    }
}

// 保存字體大小設置到本地存儲
function saveFontSize(fontSize) {
    localStorage.setItem('fontSize', fontSize);    
}

// 設置按鈕的 "active" 狀態
function setButtonActive(buttonId) {
    smallFontButton.classList.remove('active');
    mediumFontButton.classList.remove('active');
    largeFontButton.classList.remove('active');

    const activeButton = document.getElementById(buttonId);
    if (activeButton) {
        activeButton.classList.add('active');
    }
}

// 保存active的按鈕到本地存儲
function saveFontSizeButtonActive(buttonId) {
    localStorage.setItem('fontSizeaAtiveBtn', buttonId);
}

// 新增一个清除本地存储的按钮
const clearLocalStorageButton = document.getElementById('clearLocalStorageButton');
clearLocalStorageButton.addEventListener('click', function () {
    localStorage.clear();
});

function exportTables(Filename) {
    disableBootstrapStyles();
    const table2Excel = new Table2Excel('table')
    table2Excel.export(Filename);
    enableBootstrapStyles();

}
function disableBootstrapStyles() {
    // 找到 head 中的樣式表並禁用
    var stylesheets = document.head.querySelectorAll('link[rel="stylesheet"]');
    stylesheets.forEach(function (stylesheet) {
        if (stylesheet.href.includes("bootstrap.min.css")) {
            stylesheet.disabled = true;
        }
        if (stylesheet.href.includes("style.css")) {
            stylesheet.disabled = true;
        }
    });
}

function enableBootstrapStyles() {
    // 啟用禁用的樣式表
    var stylesheets = document.head.querySelectorAll('link[rel="stylesheet"]');
    stylesheets.forEach(function (stylesheet) {
        if (stylesheet.href.includes("bootstrap.min.css")) {
            stylesheet.disabled = false;
        }
        if (stylesheet.href.includes("style.css")) {
            stylesheet.disabled = false;
        }
    });
}
function numberToChinese(number) {
    var chineseNumbers = ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十'];
    var chineseTens = ['', '十', '二十', '三十', '四十', '五十', '六十', '七十', '八十', '九十'];

    if (number <= 10) {
        return chineseNumbers[number - 1];
    } else if (number <= 19) {
        return '十' + chineseNumbers[number - 11];
    } else {
        var unit = number % 10;
        var ten = Math.floor(number / 10);
        return chineseTens[ten] + (unit !== 0 ? chineseNumbers[unit - 1] : '');
    }
}




