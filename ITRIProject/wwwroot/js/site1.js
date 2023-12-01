
//返回盤查表頁面
$(document).on('click', '.returnInventoryTablePage', function () {
    // 獲取 data 屬性的值
    var basicId = $(this).data('basicid');
    var page = $(this).data('page');

    window.location.href = `/Organize/${page}/${basicId}`; 
});


//excel上傳
$(".handleUpload").on("click", function () {
    handleUpload()
});

//csv,json上傳
$(".sendDataToServer").on("click", function () {
    sendDataToServer()
});

//detail start

$(".OneMonthAuto").on("input", function () {
    OneMonthAuto(); validateDate(this); sumtotal(this)
});

$(".TwoMonthAuto").on("input", function () {
    TwoMonthAuto(); validateDate(this); TwoMonthsumtotal(this)
});

$(".sumtotal").on("input", function () {
    sumtotal(this)
});

$(".TwoMonthsumtotal").on("input", function () {
    TwoMonthsumtotal(this)
});

// MonthSubtraction1
$(document).on('click', '.MonthSubtraction1', function () {
    // 獲取 data 屬性的值
    var month = $(this).data('month');
    var addMonth = $(this).data('addmonth');
    //console.log(`month:${month}`)
    //console.log(`addMonth:${addMonth}`)
    MonthSubtraction1(month, addMonth);
});

// MonthAdd1
$(document).on('click', '.MonthAdd1', function () {
    // 獲取 data 屬性的值
    var month = $(this).data('month');
    var addMonth = $(this).data('addmonth');
    //console.log(`month:${month}`)
    //console.log(`addMonth:${addMonth}`)
    MonthAdd1(month, addMonth);
});
$(".window-close").on("click", function () {
    window.close();
});

//detail end
