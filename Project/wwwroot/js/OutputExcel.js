
function saveAsExcelFile(buffer, fileName) {
    var blob = new Blob([buffer], { type: 'application/octet-stream' });
    var url = URL.createObjectURL(blob);
    var link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(url);
}


function getColumnLabel(index) {
    var label = '';
    while (index >= 0) {
        label = String.fromCharCode(65 + (index % 26)) + label;
        index = Math.floor(index / 26) - 1;
    }
    return label;
}

function ExportExcel_Energyuse(factoryNames, EnergyNames, Transportation, Car, Units, Suppliermanages, monthcount, year, month) {
    var workbook = new ExcelJS.Workbook();
    var Energysheet = workbook.addWorksheet('能源');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A2';//A2到G3給予樣式
    var endCell = 'E3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Energysheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    var startCell1 = 'F2';
    var endCell1 = 'N3';

    for (var row = startCell1.substring(1); row <= endCell1.substring(1); row++) {
        for (var col = startCell1.charCodeAt(0); col <= endCell1.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Energysheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'E2F0D9' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }



    // 寫入資料
    Energysheet.getCell('A1').value = '請確認全廠能源使用量，例如 電力、煤、生質能源、再生能源、綠電、蒸汽…';
    Energysheet.getCell('A2').value = '場域';
    Energysheet.mergeCells('A2:A3');//合併




    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        Energysheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    Energysheet.getCell('B2').value = '能源名稱';
    Energysheet.mergeCells('B2:B3');



    // 创建下拉选项的列表值
    var EnergyNamesList = EnergyNames.map(function (item) {
        return item.Name;
    });

    EnergyNamesList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//能選種類選項
        Energysheet.getCell('B' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + EnergyNames.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('C2').value = '錶號或使用設備名稱';
    Energysheet.mergeCells('C2:C3');

    Energysheet.getCell('D2').value = '錶或設備位置';
    Energysheet.mergeCells('D2:D3');

    Energysheet.getCell('E2').value = '供應商名稱';
    Energysheet.mergeCells('E2:E3');

    var SuppliermanagesList = Suppliermanages.map(function (item) {
        return item.SupplierName;
    });

    if (SuppliermanagesList.length === 0) {
        SuppliermanagesList.push('');
    }
    SuppliermanagesList.forEach((option, index) => {
        const cell = selectdata.getCell(`F${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//供應商選項
        Energysheet.getCell('E' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$F$1:$F$' + Suppliermanages.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    //Energysheet.getCell('G2').value = '供應商地址';
    //Energysheet.mergeCells('G2:G3');

    Energysheet.getCell('F2').value = '運輸方式';
    Energysheet.mergeCells('F2:F3');


    // 创建下拉选项的列表值
    var TransportationList = Transportation.map(function (item) {
        return item.Name;
    });

    TransportationList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//交通方式
        Energysheet.getCell('F' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + Transportation.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('G2').value = '海運/空運 起迄 港口/機場';
    Energysheet.mergeCells('G2:H2');

    Energysheet.getCell('G3').value = '起點';
    Energysheet.getCell('H3').value = '迄點';

    Energysheet.getCell('I2').value = '陸運運輸資訊說明';
    Energysheet.mergeCells('I2:K2');

    Energysheet.getCell('I3').value = '車種';


    // 创建下拉选项的列表值
    var CarList = Car.map(function (item) {
        return item.Name;
    });

    CarList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//車種
        Energysheet.getCell('I' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + Car.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('J3').value = '噸數';
    Energysheet.getCell('K3').value = '燃料';

    Energysheet.getCell('L2').value = '運輸距離';
    Energysheet.mergeCells('L2:N2');

    Energysheet.getCell('L3').value = '陸運(km)';
    Energysheet.getCell('M3').value = '海運(nm)';
    Energysheet.getCell('N3').value = '空運(km)';

    Energysheet.getCell('O2').value = '數據來源資訊';
    Energysheet.mergeCells('O2:Q2');

    Energysheet.getCell('O3').value = '管理單位';
    Energysheet.getCell('P3').value = '負責人員';
    Energysheet.getCell('Q3').value = '數據來源';

    Energysheet.getCell('R2').value = '備註';
    Energysheet.mergeCells('R2:R3');

    Energysheet.getCell('S2').value = '使用(投入)量數據統計';
    Energysheet.mergeCells('S2:T2');

    Energysheet.getCell('S3').value = '單位';

    // 创建下拉选项的列表值   
    var UnitList = Units.map(function (item) {
        return item.Name;
    });
    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`E${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//單位
        Energysheet.getCell('S' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$E$1:$E$' + Units.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('T3').value = '分配比率'; 

    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }

    var field = 20;
    for (let i = 20; i < (20 + monthcount); i++) {//從x欄位開始
        for (let j = 0; j < 3; j++) {
            var column1 = getColumnLabel(field); // 获取对应的列标识
            var rowIndex = 2; // 开始的行索引
            var note1 = "";

            if (j == 0) {
                note1 = year + '/' + month + "起始日期"
            }
            else if (j == 1) {
                note1 = year + '/' + month + "結束日期"

            }
            else if (j == 2) {
                note1 = year + '/' + month + "數值"

            }

            var cell1 = Energysheet.getCell(column1 + rowIndex);

            if (!cell1.isMerged) {
                cell1.value = note1;
                Energysheet.mergeCells(column1 + rowIndex + ':' + column1 + (rowIndex + 1));//合併儲存格
            }

            field += 1;
        }
        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(" getColumnLabel(fieldss):" + (22 + (3 * monthcount)))
    var startCell2 = { col: 15, row: 2 }; // 數據來源到最後給予樣是
    var endCell2 = { col: (20 + (3 * monthcount)), row: 3 };

    for (var row = startCell2.row; row <= endCell2.row; row++) {
        for (var col = startCell2.col; col <= endCell2.col; col++) {
            var currentCell = Energysheet.getCell(row, col);

            currentCell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            currentCell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            currentCell.border = {
                top: { style: 'thin', color: { argb: '00000000' } },
                bottom: { style: 'thin', color: { argb: '00000000' } },
                left: { style: 'thin', color: { argb: '00000000' } },
                right: { style: 'thin', color: { argb: '00000000' } }
            };
        }
    }


    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '能源(1m).xlsx');
        });
}

function ExportExcel_TwoMonthEnergyuse(factoryNames, EnergyNames, Transportation, Car, Units, Suppliermanages, monthcount, year, month) {
    var workbook = new ExcelJS.Workbook();
    var Energysheet = workbook.addWorksheet('能源');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A2';//A2到G3給予樣式
    var endCell = 'E3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Energysheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    var startCell1 = 'F2';
    var endCell1 = 'N3';

    for (var row = startCell1.substring(1); row <= endCell1.substring(1); row++) {
        for (var col = startCell1.charCodeAt(0); col <= endCell1.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Energysheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'E2F0D9' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }



    // 寫入資料
    Energysheet.getCell('A1').value = '請確認全廠能源使用量，例如 電力、煤、生質能源、再生能源、綠電、蒸汽…';
    Energysheet.getCell('A2').value = '場域';
    Energysheet.mergeCells('A2:A3');//合併




    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        Energysheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    Energysheet.getCell('B2').value = '能源名稱';
    Energysheet.mergeCells('B2:B3');



    // 创建下拉选项的列表值
    var EnergyNamesList = EnergyNames.map(function (item) {
        return item.Name;
    });

    EnergyNamesList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//能選種類選項
        Energysheet.getCell('B' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + EnergyNames.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('C2').value = '錶號或使用設備名稱';
    Energysheet.mergeCells('C2:C3');

    Energysheet.getCell('D2').value = '錶或設備位置';
    Energysheet.mergeCells('D2:D3');

    Energysheet.getCell('E2').value = '供應商名稱';
    Energysheet.mergeCells('E2:E3');

    var SuppliermanagesList = Suppliermanages.map(function (item) {
        return item.SupplierName;
    });

    if (SuppliermanagesList.length === 0) {
        SuppliermanagesList.push('');
    }
    SuppliermanagesList.forEach((option, index) => {
        const cell = selectdata.getCell(`F${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//供應商選項
        Energysheet.getCell('E' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$F$1:$F$' + Suppliermanages.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    //Energysheet.getCell('G2').value = '供應商地址';
    //Energysheet.mergeCells('G2:G3');

    Energysheet.getCell('F2').value = '運輸方式';
    Energysheet.mergeCells('F2:F3');


    // 创建下拉选项的列表值
    var TransportationList = Transportation.map(function (item) {
        return item.Name;
    });

    TransportationList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//交通方式
        Energysheet.getCell('F' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + Transportation.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('G2').value = '海運/空運 起迄 港口/機場';
    Energysheet.mergeCells('G2:H2');

    Energysheet.getCell('G3').value = '起點';
    Energysheet.getCell('H3').value = '迄點';

    Energysheet.getCell('I2').value = '陸運運輸資訊說明';
    Energysheet.mergeCells('I2:K2');

    Energysheet.getCell('I3').value = '車種';


    // 创建下拉选项的列表值
    var CarList = Car.map(function (item) {
        return item.Name;
    });

    CarList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//車種
        Energysheet.getCell('I' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + Car.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('J3').value = '噸數';
    Energysheet.getCell('K3').value = '燃料';

    Energysheet.getCell('L2').value = '運輸距離';
    Energysheet.mergeCells('L2:N2');

    Energysheet.getCell('L3').value = '陸運(km)';
    Energysheet.getCell('M3').value = '海運(nm)';
    Energysheet.getCell('N3').value = '空運(km)';

    Energysheet.getCell('O2').value = '數據來源資訊';
    Energysheet.mergeCells('O2:Q2');

    Energysheet.getCell('O3').value = '管理單位';
    Energysheet.getCell('P3').value = '負責人員';
    Energysheet.getCell('Q3').value = '數據來源';

    Energysheet.getCell('R2').value = '備註';
    Energysheet.mergeCells('R2:R3');

    Energysheet.getCell('S2').value = '使用(投入)量數據統計';
    Energysheet.mergeCells('S2:T2');

    Energysheet.getCell('S3').value = '單位';

    // 创建下拉选项的列表值
    var UnitList = Units.map(function (item) {
        return item.Name;
    });
    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`E${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//單位
        Energysheet.getCell('S' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$E$1:$E$' + Units.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Energysheet.getCell('T3').value = '分配比率';


    month -= 1;
    if (month <= 0) {
        year -= 1;
        Math.abs(month += 12);
    }
    else {
        month = month;
    }

    var field = 20;
    var stylecount = 0;
    for (let i = 20; i <= (20 + monthcount); i += 2) {//從x欄位開始
        for (let j = 0; j < 3; j++) {
            var column1 = getColumnLabel(field); // 获取对应的列标识
            var rowIndex = 2; // 开始的行索引
            var note1 = "";

            if (j == 0) {
                note1 = year + '/' + month + "起始日期"
            }
            else if (j == 1) {
                note1 = year + '/' + month + "結束日期"

            }
            else if (j == 2) {
                note1 = year + '/' + month + "數值"

            }

            var cell1 = Energysheet.getCell(column1 + rowIndex);

            if (!cell1.isMerged) {
                cell1.value = note1;
                Energysheet.mergeCells(column1 + rowIndex + ':' + column1 + (rowIndex + 1));//合併儲存格
            }

            field += 1;
           
        }
        stylecount++;
        month += 2;
        if (month > 12) {
            year += 1;
            month -= 12;
        }
    }
    console.log(" getColumnLabel(fieldss):" + (20 + (3 * stylecount)))
    var startCell2 = { col: 15, row: 2 }; // 數據來源到最後給予樣是
    var endCell2 = { col: (20 + (3 * stylecount)), row: 3 };

    for (var row = startCell2.row; row <= endCell2.row; row++) {
        for (var col = startCell2.col; col <= endCell2.col; col++) {
            var currentCell = Energysheet.getCell(row, col);

            currentCell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            currentCell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            currentCell.border = {
                top: { style: 'thin', color: { argb: '00000000' } },
                bottom: { style: 'thin', color: { argb: '00000000' } },
                left: { style: 'thin', color: { argb: '00000000' } },
                right: { style: 'thin', color: { argb: '00000000' } }
            };
        }
    }


    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '能源(2m).xlsx');
        });
}

function ExportExcel_Resourceuse(factoryNames, ResourceNames, Transportation, Car, Units, supplierNames, monthcount, year, month) {
    var workbook = new ExcelJS.Workbook();
    var Resourcesheet = workbook.addWorksheet('資源');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A2';//A2到G3給予樣式
    var endCell = 'F3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Resourcesheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    var startCell1 = 'G2';
    var endCell1 = 'P3';

    for (var row = startCell1.substring(1); row <= endCell1.substring(1); row++) {
        for (var col = startCell1.charCodeAt(0); col <= endCell1.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Resourcesheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'E2F0D9' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }



    // 寫入資料
    Resourcesheet.getCell('A1').value = '請確認全廠資源使用量，例如 自來水、工業用水、地下水……';
    Resourcesheet.getCell('A2').value = '場域';
    Resourcesheet.mergeCells('A2:A3');//合併




    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        Resourcesheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    Resourcesheet.getCell('B2').value = '資源名稱';
    Resourcesheet.mergeCells('B2:B3');



    // 创建下拉选项的列表值
    var ResourceNamesList = ResourceNames.map(function (item) {
        return item.Name;
    });

    ResourceNamesList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//能選種類選項
        Resourcesheet.getCell('B' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + ResourceNames.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Resourcesheet.getCell('C2').value = '錶號或使用設備名稱';
    Resourcesheet.mergeCells('C2:C3');

    Resourcesheet.getCell('D2').value = '錶或設備位置';
    Resourcesheet.mergeCells('D2:D3');

    Resourcesheet.getCell('E2').value = '供應商名稱';
    Resourcesheet.mergeCells('E2:E3');

    var supplierNamesList = supplierNames.map(function (item) {
        return item.SupplierName;
    });

    if (supplierNamesList.length === 0) {
        supplierNamesList.push('');
    }
    supplierNamesList.forEach((option, index) => {
        const cell = selectdata.getCell(`F${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//供應商選項
        Resourcesheet.getCell('E' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$F$1:$F$' + supplierNamesList.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    //Resourcesheet.getCell('G2').value = '供應商地址';
    //Resourcesheet.mergeCells('G2:G3');

    Resourcesheet.getCell('F2').value = '運輸方式';
    Resourcesheet.mergeCells('F2:F3');


    // 创建下拉选项的列表值
    var TransportationList = Transportation.map(function (item) {
        return item.Name;
    });

    TransportationList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//交通方式
        Resourcesheet.getCell('F' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + Transportation.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Resourcesheet.getCell('G2').value = '海運/空運 起迄 港口/機場';
    Resourcesheet.mergeCells('G2:H2');

    Resourcesheet.getCell('G3').value = '起點';
    Resourcesheet.getCell('H3').value = '迄點';

    Resourcesheet.getCell('I2').value = '陸運運輸資訊說明';
    Resourcesheet.mergeCells('I2:K2');

    Resourcesheet.getCell('I3').value = '車種';


    // 创建下拉选项的列表值
    var CarList = Car.map(function (item) {
        return item.Name;
    });

    CarList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//車種
        Resourcesheet.getCell('I' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + Car.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Resourcesheet.getCell('J3').value = '噸數';
    Resourcesheet.getCell('K3').value = '燃料';

    Resourcesheet.getCell('L2').value = '運輸距離';
    Resourcesheet.mergeCells('L2:N2');

    Resourcesheet.getCell('L3').value = '陸運(km)';
    Resourcesheet.getCell('M3').value = '海運(nm)';
    Resourcesheet.getCell('N3').value = '空運(km)';

    Resourcesheet.getCell('O2').value = '數據來源資訊';
    Resourcesheet.mergeCells('O2:Q2');

    Resourcesheet.getCell('O3').value = '管理單位';
    Resourcesheet.getCell('P3').value = '負責人員';
    Resourcesheet.getCell('Q3').value = '數據來源';

    Resourcesheet.getCell('R2').value = '備註';
    Resourcesheet.mergeCells('R2:R3');

    Resourcesheet.getCell('S2').value = '使用(投入)量數據統計';
    Resourcesheet.mergeCells('S2:T2');

    Resourcesheet.getCell('S3').value = '單位';

    // 创建下拉选项的列表值
    var UnitList = Units.map(function (item) {
        return item.Name;
    });
    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`E${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//單位
        Resourcesheet.getCell('S' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$E$1:$E$' + Units.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }
    Resourcesheet.getCell('T3').value = '分配比率';



    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }

    var field = 20;
    for (let i = 20; i < (20 + monthcount); i++) {//從x欄位開始
        for (let j = 0; j < 3; j++) {
            var column1 = getColumnLabel(field); // 获取对应的列标识
            var rowIndex = 2; // 开始的行索引
            var note1 = "";

            if (j == 0) {
                note1 = year + '/' + month + "起始日期"
            }
            else if (j == 1) {
                note1 = year + '/' + month + "結束日期"

            }
            else if (j == 2) {
                note1 = year + '/' + month + "數值"

            }

            var cell1 = Resourcesheet.getCell(column1 + rowIndex);

            if (!cell1.isMerged) {
                cell1.value = note1;
                Resourcesheet.mergeCells(column1 + rowIndex + ':' + column1 + (rowIndex + 1));//合併儲存格
            }

            field += 1;
        }
        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(" getColumnLabel(fieldss):" + (22 + (3 * monthcount)))
    var startCell2 = { col: 15, row: 2 }; // 數據來源到最後給予樣是
    var endCell2 = { col: (20 + (3 * monthcount)), row: 3 };

    for (var row = startCell2.row; row <= endCell2.row; row++) {
        for (var col = startCell2.col; col <= endCell2.col; col++) {
            var currentCell = Resourcesheet.getCell(row, col);

            currentCell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            currentCell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            currentCell.border = {
                top: { style: 'thin', color: { argb: '00000000' } },
                bottom: { style: 'thin', color: { argb: '00000000' } },
                left: { style: 'thin', color: { argb: '00000000' } },
                right: { style: 'thin', color: { argb: '00000000' } }
            };
        }
    }


    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '資源(1m).xlsx');
        });
}

function ExportExcel_TwoMonthResourceuse(factoryNames, ResourceNames, Transportation, Car, Units, supplierNames, monthcount, year, month) {
    var workbook = new ExcelJS.Workbook();
    var Resourcesheet = workbook.addWorksheet('資源');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A2';//A2到G3給予樣式
    var endCell = 'F3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Resourcesheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    var startCell1 = 'G2';
    var endCell1 = 'P3';

    for (var row = startCell1.substring(1); row <= endCell1.substring(1); row++) {
        for (var col = startCell1.charCodeAt(0); col <= endCell1.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Resourcesheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'E2F0D9' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }



    // 寫入資料
    Resourcesheet.getCell('A1').value = '請確認全廠資源使用量，例如 自來水、工業用水、地下水……';
    Resourcesheet.getCell('A2').value = '場域';
    Resourcesheet.mergeCells('A2:A3');//合併




    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        Resourcesheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    Resourcesheet.getCell('B2').value = '資源名稱';
    Resourcesheet.mergeCells('B2:B3');



    // 创建下拉选项的列表值
    var ResourceNamesList = ResourceNames.map(function (item) {
        return item.Name;
    });

    ResourceNamesList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//能選種類選項
        Resourcesheet.getCell('B' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + ResourceNames.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Resourcesheet.getCell('C2').value = '錶號或使用設備名稱';
    Resourcesheet.mergeCells('C2:C3');

    Resourcesheet.getCell('D2').value = '錶或設備位置';
    Resourcesheet.mergeCells('D2:D3');

    Resourcesheet.getCell('E2').value = '供應商名稱';
    Resourcesheet.mergeCells('E2:E3');

    var supplierNamesList = supplierNames.map(function (item) {
        return item.SupplierName;
    });

    if (supplierNamesList.length === 0) {
        supplierNamesList.push('');
    }
    supplierNamesList.forEach((option, index) => {
        const cell = selectdata.getCell(`F${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//供應商選項
        Resourcesheet.getCell('E' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$F$1:$F$' + supplierNamesList.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    //Resourcesheet.getCell('G2').value = '供應商地址';
    //Resourcesheet.mergeCells('G2:G3');

    Resourcesheet.getCell('F2').value = '運輸方式';
    Resourcesheet.mergeCells('F2:F3');


    // 创建下拉选项的列表值
    var TransportationList = Transportation.map(function (item) {
        return item.Name;
    });

    TransportationList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//交通方式
        Resourcesheet.getCell('F' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + Transportation.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Resourcesheet.getCell('G2').value = '海運/空運 起迄 港口/機場';
    Resourcesheet.mergeCells('G2:H2');

    Resourcesheet.getCell('G3').value = '起點';
    Resourcesheet.getCell('H3').value = '迄點';

    Resourcesheet.getCell('I2').value = '陸運運輸資訊說明';
    Resourcesheet.mergeCells('I2:K2');

    Resourcesheet.getCell('I3').value = '車種';


    // 创建下拉选项的列表值
    var CarList = Car.map(function (item) {
        return item.Name;
    });

    CarList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//車種
        Resourcesheet.getCell('I' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + Car.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Resourcesheet.getCell('J3').value = '噸數';
    Resourcesheet.getCell('K3').value = '燃料';

    Resourcesheet.getCell('L2').value = '運輸距離';
    Resourcesheet.mergeCells('L2:N2');

    Resourcesheet.getCell('L3').value = '陸運(km)';
    Resourcesheet.getCell('M3').value = '海運(nm)';
    Resourcesheet.getCell('N3').value = '空運(km)';

    Resourcesheet.getCell('O2').value = '數據來源資訊';
    Resourcesheet.mergeCells('O2:Q2');

    Resourcesheet.getCell('O3').value = '管理單位';
    Resourcesheet.getCell('P3').value = '負責人員';
    Resourcesheet.getCell('Q3').value = '數據來源';

    Resourcesheet.getCell('R2').value = '備註';
    Resourcesheet.mergeCells('R2:R3');

    Resourcesheet.getCell('S2').value = '使用(投入)量數據統計';
    Resourcesheet.mergeCells('S2:T2');

    Resourcesheet.getCell('S3').value = '單位';

    // 创建下拉选项的列表值
    var UnitList = Units.map(function (item) {
        return item.Name;
    });
    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`E${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//單位
        Resourcesheet.getCell('S' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$E$1:$E$' + Units.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Resourcesheet.getCell('T3').value = '分配比率';

    month -= 1;
    if (month <= 0) {
        year -= 1;
        Math.abs(month += 12);
    }
    else {
        month = month;
    }

    var field = 20;
    var stylecount = 0;
    for (let i = 20; i <= (20 + monthcount); i += 2) {//從x欄位開始
        for (let j = 0; j < 3; j++) {
            var column1 = getColumnLabel(field); // 获取对应的列标识
            var rowIndex = 2; // 开始的行索引
            var note1 = "";

            if (j == 0) {
                note1 = year + '/' + month + "起始日期"
            }
            else if (j == 1) {
                note1 = year + '/' + month + "結束日期"

            }
            else if (j == 2) {
                note1 = year + '/' + month + "數值"

            }

            var cell1 = Resourcesheet.getCell(column1 + rowIndex);

            if (!cell1.isMerged) {
                cell1.value = note1;
                Resourcesheet.mergeCells(column1 + rowIndex + ':' + column1 + (rowIndex + 1));//合併儲存格
            }

            field += 1;

        }
        stylecount++;
        month += 2;
        if (month > 12) {
            year += 1;
            month -= 12;
        }
    }
    console.log(" getColumnLabel(fieldss):" + (20 + (3 * monthcount)))
    var startCell2 = { col: 15, row: 2 }; // 數據來源到最後給予樣是
    var endCell2 = { col: (20 + (3 * stylecount)), row: 3 };

    for (var row = startCell2.row; row <= endCell2.row; row++) {
        for (var col = startCell2.col; col <= endCell2.col; col++) {
            var currentCell = Resourcesheet.getCell(row, col);

            currentCell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            currentCell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            currentCell.border = {
                top: { style: 'thin', color: { argb: '00000000' } },
                bottom: { style: 'thin', color: { argb: '00000000' } },
                left: { style: 'thin', color: { argb: '00000000' } },
                right: { style: 'thin', color: { argb: '00000000' } }
            };
        }
    }


    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '資源(2m).xlsx');
        });
}


function ExportExcel_DumptreatmentOutsourcing(factoryNames, Transportation, Car, Units, monthcount, year, month, WasteName, WasteMethod, AfterUnits) {
    var workbook = new ExcelJS.Workbook();
    var DumptreatmentOutsourcingsheet = workbook.addWorksheet('廢棄物');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A2';//A2到G3給予樣式
    var endCell = 'E3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = DumptreatmentOutsourcingsheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    var startCell1 = 'F2';
    var endCell1 = 'N3';

    for (var row = startCell1.substring(1); row <= endCell1.substring(1); row++) {
        for (var col = startCell1.charCodeAt(0); col <= endCell1.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = DumptreatmentOutsourcingsheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'E2F0D9' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }



    // 寫入資料
    DumptreatmentOutsourcingsheet.getCell('A1').value = '請確認盤查邊界之全部排放，例如工業廢棄物(含回收)、生活廢棄物、廢水處理….';
    DumptreatmentOutsourcingsheet.getCell('A2').value = '場域';
    DumptreatmentOutsourcingsheet.mergeCells('A2:A3');//合併




    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        DumptreatmentOutsourcingsheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    DumptreatmentOutsourcingsheet.getCell('B2').value = '廢棄物名稱';
    DumptreatmentOutsourcingsheet.mergeCells('B2:B3');

    var WasteNameList = WasteName.map(function (item) {//場域選單
        return item.Name;
    });
    WasteNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`E${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        DumptreatmentOutsourcingsheet.getCell('B' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$E$1:$E$' + WasteName.length] //下拉選單的選項
        }
    }

    DumptreatmentOutsourcingsheet.getCell('C2').value = '廢棄物處理方式';
    DumptreatmentOutsourcingsheet.mergeCells('C2:C3');

    var WasteMethodList = WasteMethod.map(function (item) {//場域選單
        return item.WasteMethod;
    });
    WasteMethodList.forEach((option, index) => {
        const cell = selectdata.getCell(`F${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        DumptreatmentOutsourcingsheet.getCell('C' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$F$1:$F$' + WasteMethod.length] //下拉選單的選項
        }
    }


    DumptreatmentOutsourcingsheet.getCell('D2').value = '清運商名稱';
    DumptreatmentOutsourcingsheet.mergeCells('D2:D3');

    DumptreatmentOutsourcingsheet.getCell('E2').value = '最終處理地址';
    DumptreatmentOutsourcingsheet.mergeCells('E2:E3');

    DumptreatmentOutsourcingsheet.getCell('F2').value = '運輸方式';
    DumptreatmentOutsourcingsheet.mergeCells('F2:F3');


    // 创建下拉选项的列表值
    var TransportationList = Transportation.map(function (item) {
        return item.Name;
    });

    TransportationList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//交通方式
        DumptreatmentOutsourcingsheet.getCell('F' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + Transportation.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    DumptreatmentOutsourcingsheet.getCell('G2').value = '海運/空運 起迄 港口/機場';
    DumptreatmentOutsourcingsheet.mergeCells('G2:H2');

    DumptreatmentOutsourcingsheet.getCell('G3').value = '起點';
    DumptreatmentOutsourcingsheet.getCell('H3').value = '迄點';

    DumptreatmentOutsourcingsheet.getCell('I2').value = '陸運運輸資訊說明';
    DumptreatmentOutsourcingsheet.mergeCells('I2:K2');

    DumptreatmentOutsourcingsheet.getCell('I3').value = '車種';


    // 创建下拉选项的列表值
    var CarList = Car.map(function (item) {
        return item.Name;
    });

    CarList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//車種
        DumptreatmentOutsourcingsheet.getCell('I' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + Car.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    DumptreatmentOutsourcingsheet.getCell('J3').value = '噸數';
    DumptreatmentOutsourcingsheet.getCell('K3').value = '燃料';

    DumptreatmentOutsourcingsheet.getCell('L2').value = '運輸距離';
    DumptreatmentOutsourcingsheet.mergeCells('L2:N2');

    DumptreatmentOutsourcingsheet.getCell('L3').value = '陸運(km)';
    DumptreatmentOutsourcingsheet.getCell('M3').value = '海運(nm)';
    DumptreatmentOutsourcingsheet.getCell('N3').value = '空運(km)';

    DumptreatmentOutsourcingsheet.getCell('O2').value = '數據來源資訊';
    DumptreatmentOutsourcingsheet.mergeCells('O2:Q2');

    DumptreatmentOutsourcingsheet.getCell('O3').value = '管理單位';
    DumptreatmentOutsourcingsheet.getCell('P3').value = '負責人員';
    DumptreatmentOutsourcingsheet.getCell('Q3').value = '數據來源';

    DumptreatmentOutsourcingsheet.getCell('R2').value = '備註';
    DumptreatmentOutsourcingsheet.mergeCells('R2:R3');

    DumptreatmentOutsourcingsheet.getCell('S2').value = '使用(投入)量數據統計';
    DumptreatmentOutsourcingsheet.mergeCells('S2:T2');

    DumptreatmentOutsourcingsheet.getCell('S3').value = '單位';

    // 创建下拉选项的列表值
    var UnitList = Units.map(function (item) {
        return item.Name;
    });
    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//單位
        DumptreatmentOutsourcingsheet.getCell('S' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + Units.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }    

    DumptreatmentOutsourcingsheet.getCell('T3').value = '分配比率';

    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }

    var field = 20;
    for (let i = 20; i < (20 + monthcount); i++) {//從x欄位開始
        for (let j = 0; j < 3; j++) {
            var column1 = getColumnLabel(field); // 获取对应的列标识
            var rowIndex = 2; // 开始的行索引
            var note1 = "";

            if (j == 0) {
                note1 = year + '/' + month + "起始日期"
            }
            else if (j == 1) {
                note1 = year + '/' + month + "結束日期"

            }
            else if (j == 2) {
                note1 = year + '/' + month + "數值"

            }

            var cell1 = DumptreatmentOutsourcingsheet.getCell(column1 + rowIndex);

            if (!cell1.isMerged) {
                cell1.value = note1;
                DumptreatmentOutsourcingsheet.mergeCells(column1 + rowIndex + ':' + column1 + (rowIndex + 1));//合併儲存格
            }

            field += 1;
        }
        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
    }
    console.log(" getColumnLabel(fieldss):" + (20 + (3 * monthcount)))
    var startCell2 = { col: 15, row: 2 }; // 數據來源到最後給予樣是
    var endCell2 = { col: (20 + (3 * monthcount)), row: 3 };

    for (var row = startCell2.row; row <= endCell2.row; row++) {
        for (var col = startCell2.col; col <= endCell2.col; col++) {
            var currentCell = DumptreatmentOutsourcingsheet.getCell(row, col);

            currentCell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            currentCell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            currentCell.border = {
                top: { style: 'thin', color: { argb: '00000000' } },
                bottom: { style: 'thin', color: { argb: '00000000' } },
                left: { style: 'thin', color: { argb: '00000000' } },
                right: { style: 'thin', color: { argb: '00000000' } }
            };
        }
    }


    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '廢棄物(1m).xlsx');
        });
}

function ExportExcel_TwoMonthDumptreatmentOutsourcing(factoryNames, Transportation, Car, Units, monthcount, year, month, WasteName, WasteMethod) {
    var workbook = new ExcelJS.Workbook();
    var DumptreatmentOutsourcingsheet = workbook.addWorksheet('廢棄物');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A2';//A2到G3給予樣式
    var endCell = 'E3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = DumptreatmentOutsourcingsheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    var startCell1 = 'F2';
    var endCell1 = 'N3';

    for (var row = startCell1.substring(1); row <= endCell1.substring(1); row++) {
        for (var col = startCell1.charCodeAt(0); col <= endCell1.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = DumptreatmentOutsourcingsheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'E2F0D9' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }



    // 寫入資料
    DumptreatmentOutsourcingsheet.getCell('A1').value = '請確認盤查邊界之全部排放，例如工業廢棄物(含回收)、生活廢棄物、廢水處理….';
    DumptreatmentOutsourcingsheet.getCell('A2').value = '場域';
    DumptreatmentOutsourcingsheet.mergeCells('A2:A3');//合併




    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        DumptreatmentOutsourcingsheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    DumptreatmentOutsourcingsheet.getCell('B2').value = '廢棄物名稱';
    DumptreatmentOutsourcingsheet.mergeCells('B2:B3');

    var WasteNameList = WasteName.map(function (item) {//場域選單
        return item.Name;
    });
    WasteNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`E${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        DumptreatmentOutsourcingsheet.getCell('B' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$E$1:$E$' + WasteName.length] //下拉選單的選項
        }
    }

    DumptreatmentOutsourcingsheet.getCell('C2').value = '廢棄物處理方式';
    DumptreatmentOutsourcingsheet.mergeCells('C2:C3');

    var WasteMethodList = WasteMethod.map(function (item) {//場域選單
        return item.WasteMethod;
    });
    WasteMethodList.forEach((option, index) => {
        const cell = selectdata.getCell(`F${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        DumptreatmentOutsourcingsheet.getCell('C' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$F$1:$F$' + WasteMethod.length] //下拉選單的選項
        }
    }


    DumptreatmentOutsourcingsheet.getCell('D2').value = '清運商名稱';
    DumptreatmentOutsourcingsheet.mergeCells('D2:D3');

    DumptreatmentOutsourcingsheet.getCell('E2').value = '最終處理地址';
    DumptreatmentOutsourcingsheet.mergeCells('E2:E3');

    DumptreatmentOutsourcingsheet.getCell('F2').value = '運輸方式';
    DumptreatmentOutsourcingsheet.mergeCells('F2:F3');


    // 创建下拉选项的列表值
    var TransportationList = Transportation.map(function (item) {
        return item.Name;
    });

    TransportationList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//交通方式
        DumptreatmentOutsourcingsheet.getCell('F' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + Transportation.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    DumptreatmentOutsourcingsheet.getCell('G2').value = '海運/空運 起迄 港口/機場';
    DumptreatmentOutsourcingsheet.mergeCells('G2:H2');

    DumptreatmentOutsourcingsheet.getCell('G3').value = '起點';
    DumptreatmentOutsourcingsheet.getCell('H3').value = '迄點';

    DumptreatmentOutsourcingsheet.getCell('I2').value = '陸運運輸資訊說明';
    DumptreatmentOutsourcingsheet.mergeCells('I2:K2');

    DumptreatmentOutsourcingsheet.getCell('I3').value = '車種';


    // 创建下拉选项的列表值
    var CarList = Car.map(function (item) {
        return item.Name;
    });

    CarList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//車種
        DumptreatmentOutsourcingsheet.getCell('I' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + Car.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    DumptreatmentOutsourcingsheet.getCell('J3').value = '噸數';
    DumptreatmentOutsourcingsheet.getCell('K3').value = '燃料';

    DumptreatmentOutsourcingsheet.getCell('L2').value = '運輸距離';
    DumptreatmentOutsourcingsheet.mergeCells('L2:N2');

    DumptreatmentOutsourcingsheet.getCell('L3').value = '陸運(km)';
    DumptreatmentOutsourcingsheet.getCell('M3').value = '海運(nm)';
    DumptreatmentOutsourcingsheet.getCell('N3').value = '空運(km)';

    DumptreatmentOutsourcingsheet.getCell('O2').value = '數據來源資訊';
    DumptreatmentOutsourcingsheet.mergeCells('O2:Q2');

    DumptreatmentOutsourcingsheet.getCell('O3').value = '管理單位';
    DumptreatmentOutsourcingsheet.getCell('P3').value = '負責人員';
    DumptreatmentOutsourcingsheet.getCell('Q3').value = '數據來源';

    DumptreatmentOutsourcingsheet.getCell('R2').value = '備註';
    DumptreatmentOutsourcingsheet.mergeCells('R2:R3');

    DumptreatmentOutsourcingsheet.getCell('S2').value = '使用(投入)量數據統計';
    DumptreatmentOutsourcingsheet.mergeCells('S2:T2');

    DumptreatmentOutsourcingsheet.getCell('S3').value = '單位';

    // 创建下拉选项的列表值
    var UnitList = Units.map(function (item) {
        return item.Name;
    });
    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//單位
        DumptreatmentOutsourcingsheet.getCell('S' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + Units.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    DumptreatmentOutsourcingsheet.getCell('T3').value = '分配比率';

    month -= 1;
    if (month <= 0) {
        year -= 1;
        Math.abs(month += 12);
    }
    else {
        month = month;
    }

    var field = 20;
    var stylecount = 0;
    for (let i = 20; i <= (20 + monthcount); i += 2) {//從x欄位開始
        for (let j = 0; j < 3; j++) {
            var column1 = getColumnLabel(field); // 获取对应的列标识
            var rowIndex = 2; // 开始的行索引
            var note1 = "";

            if (j == 0) {
                note1 = year + '/' + month + "起始日期"
            }
            else if (j == 1) {
                note1 = year + '/' + month + "結束日期"

            }
            else if (j == 2) {
                note1 = year + '/' + month + "數值"

            }

            var cell1 = DumptreatmentOutsourcingsheet.getCell(column1 + rowIndex);

            if (!cell1.isMerged) {
                cell1.value = note1;
                DumptreatmentOutsourcingsheet.mergeCells(column1 + rowIndex + ':' + column1 + (rowIndex + 1));//合併儲存格
            }

            field += 1;

        }
        stylecount++;
        month += 2;
        if (month > 12) {
            year += 1;
            month -= 12;
        }
    }
    console.log(" getColumnLabel(fieldss):" + (20 + (3 * stylecount)))
    var startCell2 = { col: 15, row: 2 }; // 數據來源到最後給予樣是
    var endCell2 = { col: (20 + (3 * stylecount)), row: 3 };

    for (var row = startCell2.row; row <= endCell2.row; row++) {
        for (var col = startCell2.col; col <= endCell2.col; col++) {
            var currentCell = DumptreatmentOutsourcingsheet.getCell(row, col);

            currentCell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            currentCell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            currentCell.border = {
                top: { style: 'thin', color: { argb: '00000000' } },
                bottom: { style: 'thin', color: { argb: '00000000' } },
                left: { style: 'thin', color: { argb: '00000000' } },
                right: { style: 'thin', color: { argb: '00000000' } }
            };
        }
    }


    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '廢棄物(2m).xlsx');
        });
}


function ExportExcel_RefrigerantNone(factoryNames, EquipmentType, Unit, Refrigerant) {
    var workbook = new ExcelJS.Workbook();
    var RefrigerantNonesheet = workbook.addWorksheet('無進行冷媒填充');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A3';//A2到M3給予樣式
    var endCell = 'M3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = RefrigerantNonesheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    // 寫入資料
    RefrigerantNonesheet.getCell('A1').value = '請確認報告邊界之冷媒逸散排放源，例如冷氣空調、冰水機、飲水機、冰箱、冷熱衝擊機…';
    RefrigerantNonesheet.getCell('A2').value = '冷媒逸散及填充量請擇一填寫';
    RefrigerantNonesheet.getCell('A3').value = '場域';

    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        RefrigerantNonesheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    RefrigerantNonesheet.getCell('B3').value = '設備名稱';

    RefrigerantNonesheet.getCell('C3').value = '設備類型';

    // 创建下拉选项的列表值
    var EquipmentTypeList = EquipmentType.map(function (item) {
        return item.Name;
    });

    EquipmentTypeList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//設備類型
        RefrigerantNonesheet.getCell('C' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + EquipmentType.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    RefrigerantNonesheet.getCell('D3').value = '設備位置';
    RefrigerantNonesheet.getCell('E3').value = '冷媒種類';
    // 创建下拉选项的列表值
    var RefrigerantList = Refrigerant.map(function (item) {
        return item.Name;
    });

    RefrigerantList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {
        RefrigerantNonesheet.getCell('E' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + Refrigerant.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    RefrigerantNonesheet.getCell('F3').value = '設備數量';
    RefrigerantNonesheet.getCell('G3').value = '廠牌';
    RefrigerantNonesheet.getCell('H3').value = '管理單位';
    RefrigerantNonesheet.getCell('I3').value = '負責人員';
    RefrigerantNonesheet.getCell('J3').value = '備註';
    RefrigerantNonesheet.getCell('K3').value = '冷媒重量';
    RefrigerantNonesheet.getCell('L3').value = '單位';
    // 创建下拉选项的列表值
    var UnitList = Unit.map(function (item) {
        return item.Name;
    });
    if (UnitList.length === 0) {
        UnitList.push('');
    }

    UnitList.forEach((option, index) => {//分配方式
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });

    for (let i = 4; i < 1000; i++) {//分配選項
        RefrigerantNonesheet.getCell('L' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + UnitList.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    RefrigerantNonesheet.getCell('M3').value = '分配比率';

   

    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '無進行冷媒填充.xlsx');
        });
}

function ExportExcel_RefrigerantHave(factoryNames, EquipmentType, Unit, Refrigerant) {
    var workbook = new ExcelJS.Workbook();
    var RefrigerantHavesheet = workbook.addWorksheet('有進行冷媒填充');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A3';//A2到K3給予樣式
    var endCell = 'K4';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = RefrigerantHavesheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    // 寫入資料
    RefrigerantHavesheet.getCell('A1').value = '請確認報告邊界之冷媒逸散排放源，例如冷氣空調、冰水機、飲水機、冰箱、冷熱衝擊機…';
    RefrigerantHavesheet.getCell('A2').value = '冷媒逸散及填充量請擇一填寫';
    RefrigerantHavesheet.mergeCells('A3:A4');//合併
    RefrigerantHavesheet.getCell('A3').value = '場域';

    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 5; i < 1000; i++) {//場域選項
        RefrigerantHavesheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }

    RefrigerantHavesheet.getCell('B3').value = '設備名稱';
    RefrigerantHavesheet.mergeCells('B3:B4');

    RefrigerantHavesheet.getCell('C3').value = '設備類型';
    RefrigerantHavesheet.mergeCells('C3:C4');
    // 创建下拉选项的列表值
    var EquipmentTypeList = EquipmentType.map(function (item) {
        return item.Name;
    });

    EquipmentTypeList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//設備類型
        RefrigerantHavesheet.getCell('C' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + EquipmentType.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    RefrigerantHavesheet.getCell('D3').value = '冷媒種類';
    RefrigerantHavesheet.mergeCells('D3:D4');
    // 创建下拉选项的列表值
    var RefrigerantList = Refrigerant.map(function (item) {
        return item.Name;
    });

    RefrigerantList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {
        RefrigerantHavesheet.getCell('D' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + Refrigerant.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    RefrigerantHavesheet.getCell('E3').value = '數據來源資訊';
    RefrigerantHavesheet.mergeCells('E3:G3');

    RefrigerantHavesheet.getCell('E4').value = '管理單位';
    RefrigerantHavesheet.getCell('F4').value = '負責人員';
    RefrigerantHavesheet.getCell('G4').value = '數據來源';

    RefrigerantHavesheet.getCell('H3').value = '備註';
    RefrigerantHavesheet.mergeCells('H3:H4');

    RefrigerantHavesheet.getCell('I3').value = '填充(補充)量數據統計';
    RefrigerantHavesheet.mergeCells('I3:K3');

    RefrigerantHavesheet.getCell('I4').value = '填充量';
    RefrigerantHavesheet.getCell('J4').value = '單位';
    // 创建下拉选项的列表值
    var UnitList = Unit.map(function (item) {
        return item.Name;
    });
    if (UnitList.length === 0) {
        UnitList.push('');
    }

    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`D${index + 1}`);
        cell.value = option;
    });

    for (let i = 5; i < 1000; i++) {
        RefrigerantHavesheet.getCell('J' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$D$1:$D$' + UnitList.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }
    RefrigerantHavesheet.getCell('K4').value = '分配比率';

   
    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '有進行冷媒填充.xlsx');
        });
}

function ExportExcel_Workinghour(factoryNames) {
    var workbook = new ExcelJS.Workbook();
    var Workinghoursheet = workbook.addWorksheet('工作時數');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A3';//A2到G3給予樣式
    var endCell = 'G3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Workinghoursheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    // 寫入資料
    Workinghoursheet.getCell('A1').value = '不同場域需分開填寫';
    Workinghoursheet.getCell('A2').value = '正式員工、約聘人員、外派人員填寫填寫每日工作時數(小時)，總加班、總請假工時請填寫總時數(小時)';
    Workinghoursheet.getCell('A3').value = '場域';
    Workinghoursheet.getCell('B3').value = '項目';
    Workinghoursheet.getCell('C3').value = '總工時';
    Workinghoursheet.getCell('D3').value = '管理部門';
    Workinghoursheet.getCell('E3').value = '負責人員';
    Workinghoursheet.getCell('F3').value = '數據來源';
    Workinghoursheet.getCell('G3').value = '分配比率';

    // 設定 A 欄從第 4 列開始，每 5 列合併儲存格_工廠
    //工廠
    var factoryNameList = factoryNames.map(function (item) {
        return item.Name;
    });
    
    //項目
    const dataToInsert = [
        "正式員工及約聘人員(具勞保)",
        "廠商外派人員(不具勞保)",
    ];



    //for (let index = 0; index < factoryNames.length; index++) {
    //for (let row = 4; row <= (4 * dataToInsert.length + 1); row += 1) {
    //    Workinghoursheet.getCell(row, 1).value = factoryNameList[Index]
    //
    console.log(factoryNameList[0])
    console.log(factoryNameList[1])
    console.log(factoryNameList[2])
    console.log(factoryNameList[3])
    console.log(factoryNames.length)
    for (let i = 0 ,row = 4 ; row <= (2 * factoryNames.length + 3);i++, row += 2) {
        Workinghoursheet.mergeCells(`A${row}:A${row + 1}`); // merge two row
        const mergerow = Workinghoursheet.getRow(row);
        mergerow.getCell(1).value = factoryNameList[i];
        
    }



    // 設定 B 欄從第 4 列開始，每 2 列建立一個迴圈

    for (let i = 0, row = 4; row <= (2 * factoryNames.length + 3); i++, row++) {
        const number = i % 2;//讓項目每兩項一個循環
        Workinghoursheet.getCell(row, 2).value = dataToInsert[number];//固定B欄
    }




    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '工作時數.xlsx');
        });
}

function ExportExcel_Fireequipment(factoryNames, Units, fireEquipments,GHGType, monthcount, year, month) {
    var workbook = new ExcelJS.Workbook();
    var Fireequipmentsheet = workbook.addWorksheet('其他設備');//sheet名稱
    var selectdata = workbook.addWorksheet('選項');//下拉選單的sheet

    var startCell = 'A2';//A2到G3給予樣式
    var endCell = 'K3';

    for (var row = startCell.substring(1); row <= endCell.substring(1); row++) {
        for (var col = startCell.charCodeAt(0); col <= endCell.charCodeAt(0); col++) {
            var currentCell = String.fromCharCode(col) + row;
            var cell = Fireequipmentsheet.getCell(currentCell);

            cell.alignment = {
                horizontal: 'center',
                vertical: 'middle'
            };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'DEEBF7' }
            };
            cell.border = {
                top: { style: 'thin', color: { argb: '00000000' } }, // 顶部边框样式为细线，颜色为透明
                bottom: { style: 'thin', color: { argb: '00000000' } }, // 底部边框样式为细线，颜色为透明
                left: { style: 'thin', color: { argb: '00000000' } }, // 左侧边框样式为细线，颜色为透明
                right: { style: 'thin', color: { argb: '00000000' } } // 右侧边框样式为细线，颜色为透明
            };
        }
    }

    // 寫入資料
    Fireequipmentsheet.getCell('A1').value = '請確認盤查時間內是否有新購或回填的消防設備支數，例如CO2(5p、10p、15p 分開)滅火器、BC型乾粉滅火器、KBC型乾粉滅火器、FM200、SF6氣體斷路器…';
    Fireequipmentsheet.getCell('A2').value = '場域';
    Fireequipmentsheet.mergeCells('A2:A3');//合併

    // 创建下拉选项的列表值
    var factoryNameList = factoryNames.map(function (item) {//場域選單
        return item.Name;
    });
    console.log(factoryNames.length)
    factoryNameList.forEach((option, index) => {
        const cell = selectdata.getCell(`A${index + 1}`);//把場域入選項的sheet
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//場域選項
        Fireequipmentsheet.getCell('A' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$A$1:$A$' + factoryNames.length] //下拉選單的選項
        }
    }


    Fireequipmentsheet.getCell('B2').value = '設備名稱';
    Fireequipmentsheet.mergeCells('B2:B3');
    // 创建下拉选项的列表值
    var fireEquipmentsList = fireEquipments.map(function (item) {
        return item.Name;
    });
    fireEquipmentsList.forEach((option, index) => {
        const cell = selectdata.getCell(`B${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {
        Fireequipmentsheet.getCell('B' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$B$1:$B$' + fireEquipments.length] //下拉選單的選項
        }
    }

    Fireequipmentsheet.getCell('C2').value = '溫室氣體種類';
    Fireequipmentsheet.mergeCells('C2:C3');
    // 创建下拉选项的列表值
    var ghptypeList = GHGType.map(function (item) {
        return item.Name;
    });
    ghptypeList.forEach((option, index) => {
        const cell = selectdata.getCell(`C${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {
        Fireequipmentsheet.getCell('C' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$C$1:$C$' + GHGType.length] //下拉選單的選項
        }
    }

    Fireequipmentsheet.getCell('D2').value = '數據來源資訊';
    Fireequipmentsheet.mergeCells('D2:F2');

    Fireequipmentsheet.getCell('D3').value = '管理單位';
    Fireequipmentsheet.getCell('E3').value = '負責人員';
    Fireequipmentsheet.getCell('F3').value = '數據來源';

    Fireequipmentsheet.getCell('G2').value = '備註';
    Fireequipmentsheet.mergeCells('G2:G3');



    Fireequipmentsheet.getCell('H2').value = '使用(投入)量數據統計';
    Fireequipmentsheet.mergeCells('H2:I2');

    Fireequipmentsheet.getCell('H3').value = '單位';

    // 创建下拉选项的列表值
    var UnitList = Units.map(function (item) {
        return item.Name;
    });
    UnitList.forEach((option, index) => {
        const cell = selectdata.getCell(`E${index + 1}`);
        cell.value = option;
    });
    for (let i = 4; i < 1000; i++) {//單位
        Fireequipmentsheet.getCell('H' + i).dataValidation = {
            type: 'list',
            allowBlank: true,
            formulae: ['選項!$E$1:$E$' + Units.length] // 假设有效值列表在 Sheet2 表的 A1:A10 单元格范围内
        }
    }

    Fireequipmentsheet.getCell('I3').value = '分配比率';

    

    if (month == 0) {
        year -= 1;
        var month = 12;
    }
    else {
        var month = month;
    }

    var field = 9;
    for (let i = 9; i < (9 + monthcount); i++) {//從x欄位開始
        for (let j = 0; j < 3; j++) {
            var column1 = getColumnLabel(field); // 获取对应的列标识
            var rowIndex = 2; // 开始的行索引
            var note1 = "";

            if (j == 0) {
                continue;
            }
            else if (j == 1) {
                continue;
            }
            else if (j == 2) {
                note1 = year + '/' + month + "數值"

            }

            var cell1 = Fireequipmentsheet.getCell(column1 + rowIndex);

            if (!cell1.isMerged) {
                cell1.value = note1;
                Fireequipmentsheet.mergeCells(column1 + rowIndex + ':' + column1 + (rowIndex + 1));//合併儲存格
            }

            field += 1;
        }
        month++;
        if (month > 12) {
            year += 1;
            month = 1;
        }
        var startCell2 = { col: 9, row: 2 }; // 數據來源到最後給予樣是
        var endCell2 = { col: (9 + monthcount), row: 3 };

        for (var row = startCell2.row; row <= endCell2.row; row++) {
            for (var col = startCell2.col; col <= endCell2.col; col++) {
                var currentCell = Fireequipmentsheet.getCell(row, col);

                currentCell.alignment = {
                    horizontal: 'center',
                    vertical: 'middle'
                };
                currentCell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: 'DEEBF7' }
                };
                currentCell.border = {
                    top: { style: 'thin', color: { argb: '00000000' } },
                    bottom: { style: 'thin', color: { argb: '00000000' } },
                    left: { style: 'thin', color: { argb: '00000000' } },
                    right: { style: 'thin', color: { argb: '00000000' } }
                };
            }
        }
    }

    // 保存為 Excel 檔案
    workbook.xlsx.writeBuffer()
        .then(function (buffer) {
            saveAsExcelFile(buffer, '其他設備.xlsx');
        });
}



