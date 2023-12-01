function ExportJSON_Energyuse(monthcount, year, month) {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "能源名稱": "", "錶號或使用設備名稱": "", "錶或設備位置": "", "供應商名稱": "", "運輸方式": "", "起點": "",
            "迄點": "", "車種": "", "噸數": "", "燃料": "", "陸運(km)": "", "海運(nm)": "", "空運(km)": "", "管理單位": "",
            "負責人員": "", "數據來源": "", "備註": "", "單位": "","分配比率": ""
        }
    ];
    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }
    for (var i = 0; i < monthcount; i++) {

        data[0][year + '/' + month + "起始日期"] = "";
        data[0][year + '/' + month + "結束日期"] = "";
        data[0][year + '/' + month + "數值"] = "";

        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }
        
    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "能源(1m).json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_TwoMonthEnergyuse(monthcount, year, month) {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "能源名稱": "", "錶號或使用設備名稱": "", "錶或設備位置": "", "供應商名稱": "", "運輸方式": "", "起點": "",
            "迄點": "", "車種": "", "噸數": "", "燃料": "", "陸運(km)": "", "海運(nm)": "", "空運(km)": "", "管理單位": "",
            "負責人員": "", "數據來源": "", "備註": "", "單位": "",  "分配比率": ""
        }
    ];
    month -= 1;
    if (month <= 0) {
        year -= 1;
        Math.abs(month += 12);
    }
    else {
        month = month;
    }
    for (var i = 0; i <= monthcount; i += 2) {

        data[0][year + '/' + month + "起始日期"] = "";
        data[0][year + '/' + month + "結束日期"] = "";
        data[0][year + '/' + month + "數值"] = "";

        month += 2;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "能源(2m).json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_Resourceuse(monthcount, year, month) {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "資源名稱": "", "錶號或使用設備名稱": "", "錶或設備位置": "", "供應商名稱": "", "運輸方式": "", "起點": "",
            "迄點": "", "車種": "", "噸數": "", "燃料": "", "陸運(km)": "", "海運(nm)": "", "空運(km)": "", "管理單位": "",
            "負責人員": "", "數據來源": "", "備註": "", "單位": "", "分配比率": ""
        }
    ];
    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }
    for (var i = 0; i < monthcount; i++) {

        data[0][year + '/' + month + "起始日期"] = "";
        data[0][year + '/' + month + "結束日期"] = "";
        data[0][year + '/' + month + "數值"] = "";

        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "資源(1m).json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_TwoMonthResourceuse(monthcount, year, month) {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "資源名稱": "", "錶號或使用設備名稱": "", "錶或設備位置": "", "供應商名稱": "", "運輸方式": "", "起點": "",
            "迄點": "", "車種": "", "噸數": "", "燃料": "", "陸運(km)": "", "海運(nm)": "", "空運(km)": "", "管理單位": "",
            "負責人員": "", "數據來源": "", "備註": "", "單位": "","分配比率": ""
        }
    ];
    month -= 1;
    if (month <= 0) {
        year -= 1;
        Math.abs(month += 12);
    }
    else {
        month = month;
    }
    for (var i = 0; i <= monthcount; i += 2) {

        data[0][year + '/' + month + "起始日期"] = "";
        data[0][year + '/' + month + "結束日期"] = "";
        data[0][year + '/' + month + "數值"] = "";

        month += 2;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "資源(2m).json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_RefrigerantNone() {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "設備名稱": "", "設備類型": "", "設備位置": "", "冷媒種類": "", "設備數量": "", "廠牌": "",
            "管理單位": "", "負責人員": "", "備註": "", "冷媒重量": "", "單位": "", "分配比率": ""
        }
    ];
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "無進行冷媒填充.json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_RefrigerantHave() {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "設備名稱": "", "設備類型": "", "冷媒種類": "", "管理單位": "", "負責人員": "", "數據來源":"", "備註": "", "填充量": "", "單位": "", "分配比率": ""
        }
    ];
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "有進行冷媒填充.json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_Workinghour() {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "項目": "", "總工時": "", "管理部門": "", "負責人員": "", "數據來源": "", "分配比率": ""
        }
    ];
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "工作時數.json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_Fireequipment(monthcount, year, month) {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "設備名稱": "", "溫室氣體種類": "", "管理單位": "", "負責人員": "", "數據來源": "", "備註": "",
            "單位": "",  "分配比率": ""
        }
    ];
    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }
    for (var i = 0; i < monthcount; i++) {
        data[0][year + '/' + month + "數值"] = "";

        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "其他設備.json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_DumptreatmentOutsourcing(monthcount, year, month) {
    // 假設這是要匯出的資料    
    var data = [
        {
            "場域": "", "空水廢名稱": "", "空水廢處理方式": "", "清運商名稱": "", "最終處理地址": "", "運輸方式": "", "起點": "",
            "迄點": "", "車種": "", "噸數": "", "燃料": "", "陸運(km)": "", "海運(nm)": "", "空運(km)": "", "管理單位": "",
            "負責人員": "", "數據來源": "", "備註": "", "單位": "",  "分配比率": ""
        }
    ];
    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }
    for (var i = 0; i < monthcount; i++) {

        data[0][year + '/' + month + "起始日期"] = "";
        data[0][year + '/' + month + "結束日期"] = "";
        data[0][year + '/' + month + "數值"] = "";

        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "廢棄物(1m).json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}

function ExportJSON_TwoMonthDumptreatmentOutsourcing(monthcount, year, month) {
    // 假設這是要匯出的資料
    var data = [
        {
            "場域": "", "空水廢名稱": "", "空水廢處理方式": "", "清運商名稱": "", "最終處理地址": "", "運輸方式": "", "起點": "",
            "迄點": "", "車種": "", "噸數": "", "燃料": "", "陸運(km)": "", "海運(nm)": "", "空運(km)": "", "管理單位": "",
            "負責人員": "", "數據來源": "", "備註": "", "單位": "", "分配比率": ""
        }
    ];
    month -= 1;
    if (month <= 0) {
        year -= 1;
        Math.abs(month += 12);
    }
    else {
        month = month;
    }
    for (var i = 0; i <= monthcount; i += 2) {

        data[0][year + '/' + month + "起始日期"] = "";
        data[0][year + '/' + month + "結束日期"] = "";
        data[0][year + '/' + month + "數值"] = "";

        month += 2;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(data);
    // 將資料轉換成 CSV 格式的字串
    var JsonContent = "[\n";

    data.forEach(function (rowArray, index, array) {
        var row = JSON.stringify(rowArray);
        JsonContent += row;
        if (index < data.length - 1) {
            JsonContent += ",";
        } else {
            JsonContent += '\n]'
        }

    });
    var blob = new Blob([JsonContent]);
    // 建立一個隱藏的連結，並設定 href 屬性為包含 CSV 資料的 data URI
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.setAttribute("download", "廢棄物(2m).json");

    // 將連結加到頁面中，並自動點擊連結以觸發下載
    document.body.appendChild(link);
    link.click();
}