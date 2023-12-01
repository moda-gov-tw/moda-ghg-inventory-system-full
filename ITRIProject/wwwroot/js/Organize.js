
$(document).on('click', '.ViewBasicdata-btn', function () {
    // 獲取 data 屬性的值
    var basicId = $(this).data('basicid');

    // 執行相應的操作，這裡可以調用 DeleteInventory 函數
    var url = '/Organize/Group/' + basicId;

    // 打开弹出窗口，设置窗口大小和其他属性
    var popup = window.open(url, 'PopupWindow', 'width=800,height=600');

    // 阻止默认链接点击行为
    return false;
});
$(document).on('click', '.RevokeSignOff-btn', function () {
    // 獲取 data 屬性的值
    var basicId = $(this).data('basicid');
    var inventory = $(this).data('inventory');

    // 執行相應的操作，這裡可以調用 DeleteInventory 函數
    RevokeSignOff(basicId, inventory);
});
// 使用 jQuery 的 on 方法綁定點擊事件
$(document).on('click', '.delete-button', function () {
    // 獲取 data 屬性的值
    var basicId = $(this).data('basicid');
    var inventory = $(this).data('inventory');

    // 執行相應的操作，這裡可以調用 DeleteInventory 函數
    DeleteInventory(basicId, inventory);
});
function DeleteInventory(id, Inventory) {
    var confirmed = window.confirm("確定要刪除[" + Inventory + "]盤查表嗎？刪除後無法復原!");
    if (!confirmed) {
        return;
    }
    $.ajax({
        url: "/Organize/Delete/" + id,
        type: "POST",
        processData: false,
        contentType: false,
        //data: formData, // 將表單數據和驗證碼傳遞到後端
        dataType: "json",
        success: function (response) {
            if (response.success) {
                window.location.href = "/Organize/Details";
            } else {
                // 登入失敗，處理錯誤回應
                alert(response.error);
            }
        },
    });
}
function RevokeSignOff(id, Inventory) {
    var confirmed = window.confirm("確定要撤回[" + Inventory + "]盤查表的簽核嗎？");
    if (!confirmed) {
        return;
    }
    $.ajax({
        url: "/Organize/RevokeSignOff/" + id,
        type: "POST",
        processData: false,
        contentType: false,
        //data: formData, // 將表單數據和驗證碼傳遞到後端
        dataType: "json",
        success: function (response) {
            if (response.success) {
                window.location.href = "/Organize/Details";
            } else {
                // 登入失敗，處理錯誤回應
                alert(response.error);
            }
        },
    });
} 