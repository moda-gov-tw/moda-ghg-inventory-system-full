// var data = [], rowIds = [];
// 	function getBills() {
// 		var rowCount = 10;
// 		for (var i = 0; i < rowCount; i ++) {
// 			data.push({
// 				sid: i,
// 			})
// 		}
// 		$("#proadd").jqGrid("clearGridData").jqGrid('setGridParam',{data: data || []}).trigger('reloadGrid');
// 	}
// 	function pihaoValidate(value, column) {
// 		var now = new Date, 
// 			month = (now.getMonth() + 1) < 10 ? "0" + (now.getMonth() + 1) : now.getMonth() + 1,
// 			day = now.getDate() < 10 ? "0" + now.getDate() : now.getDate(),
// 			ymd = now.getFullYear() + "" + month + "" + day;
// 		if(value && value.indexOf(ymd) >= 0) {
// 			return [true, ''];
// 		} else {
// 			return [false, '批号必须当前日期:年月日开头!'];
// 		}
// 	}
! function(document, window, $) {
 
    
    "use strict";
    var Site = window.Site;
    $(document).ready(function($) {
        //getBills()
        }), jsGrid.setDefaults({
            tableClass: "jsgrid-table table table-striped table-hover"
        }), jsGrid.setDefaults("text", {
            _createTextBox: function() {
                return $("<input>").attr("type", "text").attr("class", "")
            }
        }), jsGrid.setDefaults("number", {
            _createTextBox: function() {
                return $("<input>").attr("type", "number").attr("class", "")
            }
        }), jsGrid.setDefaults("textarea", {
            _createTextBox: function() {
                return $("<input>").attr("type", "textarea").attr("class", "materialize-textarea")
            }
        }), jsGrid.setDefaults("control", {
            _createGridButton: function(cls, tooltip, clickHandler) {
                var grid = this._grid;
                return $("<button>").addClass(this.buttonClass).addClass(cls).attr({
                    type: "button",
                    title: tooltip
                }).on("click", function(e) {
                    clickHandler(grid, e)
                })
            }
        }), jsGrid.setDefaults("select", {
            _createSelect: function() {
                var $result = $("<select>").attr("class", ""),
                    valueField = this.valueField,
                    textField = this.textField,
                    selectedIndex = this.selectedIndex;
                return $.each(this.items, function(index, item) {
                    var value = valueField ? item[valueField] : index,
                        text = textField ? item[textField] : item,
                        $option = $("<option>").attr("value", value).text(text).appendTo($result);
                    $option.prop("selected", selectedIndex === index)
                }), $result
            }
        }),
        function() {
            $("#proadd").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                data: db.pro,
                fields: [
                    { name: "編號", type: "text",  width: 150, align: "center" },
                    { name: "商品名稱", type: "select", items: db.countries, valueField: "Id", textField: "Name" },
                    { name: "物質類別",type: "select", items: db.prowz, valueField: "Id", textField: "Name" , align: "center" },
                    { name: "HFCs冷媒編號", type: "text", width: 100, align: "center" },
                    { name: "客戶名稱", type: "text",editable:true,editrules:{required:true}, width: 100, align: "center" },
                    { name: "銷售量", type: "text",editable:true,editrules:{custom:true,required:false},width: 100, align: "center" },
                    { name: "備註", type: "text", width: 100, align: "center" },
                   
                ]
            });
        }(),
        function() {
            $("#proadd2").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                data: db.pro2,
                fields: [
                    { name: "編號", type: "text",  width: 150, align: "center",editable:false,},
                    { name: "商品名稱", type: "select", items: db.countries, valueField: "Id", textField: "Name" ,editable:false,},
                    { name: "物質類別",type: "select", items: db.prowz, valueField: "Id", textField: "Name" , align: "center" ,editable:false,},
                    { name: "HFCs冷媒編號", type: "text", width: 100, align: "center" ,editable:false,},
                    { name: "進口量", type: "text", width: 100, align: "center" ,editable:true,},
                    { name: "出口量", type: "text", width: 100, align: "center" ,editable:true,},
                    { name: "客戶名稱", type: "text",editrules:{required:true}, width: 100, align: "center" ,editable:false,},
                    { name: "銷售量", type: "text",editrules:{custom:true,required:false},width: 100, align: "center",editable:false, },
                    { name: "備註", type: "text", width: 100, align: "center",editable:true, },
                   
                ]
            });
        }(),
        function() {
            $("#proadd3").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                data: db.pro3,
                fields: [
                    { name: "編號", type: "text",  width: 150, align: "center",editable:false,},
                    { name: "試驗/ 實際長期購買", type: "select", items: db.prosy, valueField: "Id", textField: "Name" ,editable:true,},
                    { name: "商品名稱", type: "select", items: db.countries, valueField: "Id", textField: "Name" ,editable:false,},
                    { name: "物質類別",type: "select", items: db.prowz, valueField: "Id", textField: "Name" , align: "center" ,editable:false,},
                    { name: "HFCs冷媒編號", type: "text", width: 100, align: "center" ,editable:false,},
                    { name: "GWP值", type: "text", width: 100, align: "center" ,editable:true,},
                    { name: "銷售量", type: "text", width: 100, align: "center" ,editable:true,},
                    { name: "使用用途",  type: "select", items: db.yongtu, valueField: "Id", textField: "Name" ,editable:true,},
                    { name: "替代案例介紹說明", type: "text",width: 100, align: "center",editable:false, },
                    { name: "備註", type: "text", width: 100, align: "center",editable:true, },
           
                ]
            });
        }(),
        function() {
            $("#proadd4").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                data: db.pro4,
                fields: [
                    { name: "編號",type: "text",  width: 150, align: "center",},
                    { name: "HFCs冷媒編號", type: "select", items: db.prohfcs, valueField: "Id", textField: "Name" },
                    { name: "年度回收量",type: "text", width: 100, align: "center"},
                    { name: "使用回收機台數",type: "text", width: 100, align: "center" },
                    { name: "廠內回收或廠外回收",type: "select", items: db.progc, valueField: "Id", textField: "Name"  },
                 
                ]
            });
        }(),
        function() {
   
            $("#bproadd").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                pagerContainer: "#externalPager",
            pagerFormat: "{pageIndex} &nbsp;&nbsp; {first} {prev} {pages} {next} {last} &nbsp;&nbsp; total pages: {pageCount}",
            pagePrevText: "<",
            pageNextText: ">",
            pageFirstText: "<<",
            pageLastText: ">>",
            pageNavigatorNextText: "&#8230;",
            pageNavigatorPrevText: "&#8230;",
                data: db.bpro, 
                fields: [
                    { name: "編號", type: "rownumbers",  width: 50, align: "center" },
                    { name: "商品名稱", type: "select", items: db.countries, valueField: "Id", textField: "Name" },
                    { name: "物質類別",type: "text",editable:false, items: db.prowz, valueField: "Id", textField: "Name" , align: "center" },
                    { name: "HFCs冷媒編號", type: "text", width: 100, align: "center" },
                    { name: "採購量", type: "text",editable:true,editrules:{custom:true,required:false},width: 100, align: "center" },
                    { name: "國內/國外廠商", type: "select", items: db.bus, valueField: "Id", textField: "Name" },
                    { name: "供應廠商名稱", type: "text",editable:true,editrules:{required:true}, width: 100, align: "center" },
                    { name: "備註", type: "text", width: 100, align: "center" },

                ]
            });
        }(),
        function() {
           
            $("#bproadd2").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                pagerContainer: "#externalPager",
            pagerFormat: "{pageIndex} &nbsp;&nbsp; {first} {prev} {pages} {next} {last} &nbsp;&nbsp; total pages: {pageCount}",
            pagePrevText: "<",
            pageNextText: ">",
            pageFirstText: "<<",
            pageLastText: ">>",
            pageNavigatorNextText: "&#8230;",
            pageNavigatorPrevText: "&#8230;",
                data: db.bpro2,
                fields: [
                    { name: "編號", type: "rownumbers",  width: 50, align: "center" },
                    { name: "用途", type: "select",width: 150,  items: db.yongtu, valueField: "Id", textField: "Name" },
                    { name: "設備類別", type: "select", items: db.sbnb, valueField: "Id", textField: "Name" },
                    { name: "物質類別", type: "select", items: db.prowz, valueField: "Id", textField: "Name" },
                    { name: "HFCs冷媒編號", type: "text", width: 100, align: "center" },
                    { name: "使用量", type: "text",editable:true,editrules:{custom:true,required:false},width: 100, align: "center" },
                    { name: "備註", type: "text", width: 100, align: "center" },
              
                ]
            });
        }(),
        function() {
           
            $("#bproadd3").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                pagerContainer: "#externalPager",
            pagerFormat: "{pageIndex} &nbsp;&nbsp; {first} {prev} {pages} {next} {last} &nbsp;&nbsp; total pages: {pageCount}",
            pagePrevText: "<",
            pageNextText: ">",
            pageFirstText: "<<",
            pageLastText: ">>",
            pageNavigatorNextText: "&#8230;",
            pageNavigatorPrevText: "&#8230;",
                data: db.bpro3,
                fields: [
                    { name: "編號", type: "rownumbers",  width: 50, align: "center" },
                    { name: "商品名稱", type: "select",editable:false, items: db.countries, valueField: "Id", textField: "Name" },
                    { name: "物質類別", type: "select",editable:false, items: db.prowz, valueField: "Id", textField: "Name" },
                    { name: "HFCs冷媒編號", type: "text", width: 100, align: "center" },
                    { name: "本公司HFCs製程利用率（％）", type: "text",editable:true,editrules:{custom:true,required:false},width: 100, align: "center" },

                ]
            });
        }(),
        function() {
           
            $("#bproadd4").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                pagerContainer: "#externalPager",
            pagerFormat: "{pageIndex} &nbsp;&nbsp; {first} {prev} {pages} {next} {last} &nbsp;&nbsp; total pages: {pageCount}",
            pagePrevText: "<",
            pageNextText: ">",
            pageFirstText: "<<",
            pageLastText: ">>",
            pageNavigatorNextText: "&#8230;",
            pageNavigatorPrevText: "&#8230;",
                data: db.bpro4,
                fields: [
                    { name: "編號", type: "rownumbers",  width: 50, align: "center" },
                    { name: "商品名稱", type: "select",editable:false, items: db.countries, valueField: "Id", textField: "Name" },
                    { name: "物質類別", type: "select",editable:false, items: db.prowz, valueField: "Id", textField: "Name" },
                    { name: "HFCs冷媒編號", type: "text", width: 100, align: "center" },
                    { name: "採購量", type: "text", width: 100, align: "center" },
                    { name: "使用量", type: "text", width: 100, align: "center" },
                    { name: "庫存量", type: "text", width: 100, align: "center" },
     
                ]
            });
        }(),
        function() {
           
            $("#bproadd5").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                pagerContainer: "#externalPager",
            pagerFormat: "{pageIndex} &nbsp;&nbsp; {first} {prev} {pages} {next} {last} &nbsp;&nbsp; total pages: {pageCount}",
            pagePrevText: "<",
            pageNextText: ">",
            pageFirstText: "<<",
            pageLastText: ">>",
            pageNavigatorNextText: "&#8230;",
            pageNavigatorPrevText: "&#8230;",
                data: db.bpro5,
                fields: [
        
                    { name: "編號", type: "rownumbers",  width: 50, align: "center" },
                    { name: "轉用替代品之設備類別", type: "text", width: 150, align: "center" },
                    { name: "商品名稱", type: "select",editable:false, items: db.countries, valueField: "Id", textField: "Name" },
                    { name: "物質類別", type: "select",editable:false, items: db.prowz, valueField: "Id", textField: "Name" },
                    { name: "原使用HFCs冷媒編號", type: "text", width: 100, align: "center" },
                    { name: "原使用HFCs數量（公噸）", type: "text", width: 100, align: "center" },
                    { name: "替代品", type: "select",editable:false, items: db.tidai, valueField: "Id", textField: "Name" },
                    { name: "替代案例介紹說明", type: "text", width: 100, align: "center" },
                    { name: "備註", type: "text", width: 100, align: "center" },
      
                ]
            });
        }(),
        function() {
           
            $("#bproadd6").jsGrid({
                height: "500px",
                width: "100%",
                filtering: 0,
                editing: 0,
                sorting: 0,
                paging: 0,
                autoload: !0,
                control:0,
                pageSize: 15,
                pageButtonCount: 5,
                pagerContainer: "#externalPager",
            pagerFormat: "{pageIndex} &nbsp;&nbsp; {first} {prev} {pages} {next} {last} &nbsp;&nbsp; total pages: {pageCount}",
            pagePrevText: "<",
            pageNextText: ">",
            pageFirstText: "<<",
            pageLastText: ">>",
            pageNavigatorNextText: "&#8230;",
            pageNavigatorPrevText: "&#8230;",
                data: db.bpro6,
                fields: [
        
                    { name: "編號", type: "rownumbers",  width: 50, align: "center" },
                    { name: "HFCS冷媒編號", type: "select",editable:false, items: db.prohfcs, valueField: "Id", textField: "Name" },
                    { name: "回收量(公噸)",  type: "text", width: 100, align: "center" },
                    { name: "使用回收機台數", type: "text", width: 100, align: "center" },
        
  
                ]
            });
        }(),
        function() {
            $("#customgrid").jsGrid({
                height: "90%",
                width: "100%",
         
                inserting: true,
                editing: true,
                sorting: true,
                paging: true,
         
                data: db.users,
         
                fields: [
                    { name: "Account", width: 150, align: "center" },
                    { name: "Name", type: "text" },
                    { name: "RegisterDate", type: "myDateField", width: 100, align: "center" },
                    { type: "control", editButton: false, modeSwitchButton: false }
                ]
            });
        }(),
        function() {
            $("#basicgrid").jsGrid({
                height: "500px",
                width: "100%",
                filtering: !0,
                editing: !0,
                sorting: !0,
                paging: !0,
                autoload: !0,
                pageSize: 15,
                pageButtonCount: 5,
                deleteConfirm: "Do you really want to delete the client?",
                controller: db,
                fields: [{
                    name: "Name",
                    type: "text",
                    width: 150
                }, {
                    name: "Age",
                    type: "number",
                    width: 70
                }, {
                    name: "Address",
                    type: "text",
                    width: 200
                }, {
                    name: "Country",
                    type: "select",
                    items: db.countries,
                    valueField: "Id",
                    textField: "Name"
                }, {
                    type: "control"
                }]
            })
        }(),
        function() {
            $("#staticgrid").jsGrid({
                height: "500px",
                width: "100%",
                sorting: !0,
                paging: !0,
                data: db.clients,
                fields: [{
                    name: "Name",
                    type: "text",
                    width: 150
                }, {
                    name: "Age",
                    type: "number",
                    width: 70
                }, {
                    name: "Address",
                    type: "text",
                    width: 200
                }, {
                    name: "Country",
                    type: "select",
                    items: db.countries,
                    valueField: "Id",
                    textField: "Name"
                }]
            })
        }(),

        function() {
            $("#exampleSorting").jsGrid({
                height: "500px",
                width: "100%",
                autoload: !0,
                selecting: !1,
                controller: db,
                fields: [{
                    name: "Name",
                    type: "text",
                    width: 150
                }, {
                    name: "Age",
                    type: "number",
                    width: 50
                }, {
                    name: "Address",
                    type: "text",
                    width: 200
                }, {
                    name: "Country",
                    type: "select",
                    items: db.countries,
                    valueField: "Id",
                    textField: "Name"
                }]
            }), $("#sortingField").on("change", function() {
                var field = $(this).val();
                $("#exampleSorting").jsGrid("sort", field)
            })
        }(),

        function() {
            var MyDateField = function(config) {
                jsGrid.Field.call(this, config)
            };
            MyDateField.prototype = new jsGrid.Field({
                sorter: function(date1, date2) {
                    return new Date(date1) - new Date(date2)
                },
                itemTemplate: function(value) {
                    return new Date(value).toDateString()
                },
                insertTemplate: function() {
                    if (!this.inserting) return "";
                    var $result = this.insertControl = this._createTextBox();
                    return $result
                },
                editTemplate: function(value) {
                    if (!this.editing) return this.itemTemplate(value);
                    var $result = this.editControl = this._createTextBox();
                    return $result.val(value), $result
                },
                insertValue: function() {
                    return this.insertControl.datepicker("getDate")
                },
                editValue: function() {
                    return this.editControl.datepicker("getDate")
                },
                _createTextBox: function() {
                    return $("<input>").attr("type", "text").addClass("form-control input-sm").datepicker({
                        autoclose: !0
                    })
                }
            }), jsGrid.fields.myDateField = MyDateField, $("#exampleCustomGridField").jsGrid({
                height: "500px",
                width: "100%",
                inserting: !0,
                editing: !0,
                sorting: !0,
                paging: !0,
                data: db.users,
                fields: [{
                    name: "Account",
                    width: 150,
                    align: "center"
                }, {
                    name: "Name",
                    type: "text"
                }, {
                    name: "RegisterDate",
                    type: "myDateField",
                    width: 100,
                    align: "center"
                }, {
                    type: "control",
                    editButton: !1,
                    modeSwitchButton: !1
                }]
            })
        }()
}(document, window, jQuery);