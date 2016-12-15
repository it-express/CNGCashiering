$(document).ready(function () {
    function Validate(st) {
        var err = "";

        if (st.StockTransferDate == "") {
            err = "Stock transfer date is required.";
        }
        else if (st.JobOrderNo == "") {
            err = "Job order no is required.";
        }
        else if (st.UnitPlateNo == "") {
            err = "Unit plate no is required.";
        }
        else if (st.JobOrderDate == "") {
            err = "Job order date is required.";
        }
        else if (st.OdometerReading == "") {
            err = "Odometer reading is required.";
        }
        else if (st.Items.length == 0) {
            err = "Please select item/s.";
        }
        else {
            $.each(st.Items, function (key, value) {
                item = value;

                if (parseInt(item.Quantity) > parseInt(item.QuantityOnHand)) {
                    err = "Item Code:" + item.Code + "has insufficient quantity on hand";
                }
            });
        }

        return err;
    }

    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        var st = new Object();
        st.No = $('#lblStNo').text();
        st.StockTransferDate = $('#lblStDate').val();
        st.CompanyTo = $('#StockTransfer_CompanyTo').val();
        st.JobOrderNo = $('#txtJobOrderNo').val();
        // st.UnitPlateNo = $('#UnitPlateNo').val().text();
        st.UnitPlateNo = $("#UnitPlateNo option:selected").text();
        st.JobOrderDate = $('#txtJobOrderDate').val();
        st.OdometerReading = $('#txtOdometerReading').val();
        st.DriverName = $('#txtDriverName').val();

        st.ReportedBy = $('#txtReportedBy').val();
        st.CheckedBy = $('#txtCheckedBy').val();

        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.ItemId = $this.data("item-id");
            item.Quantity = $this.find(".txtQuantity").val();
            item.SerialNo = $this.find(".txtSerialNo").val();
            item.Type = $this.find(".selType").val();
            item.QuantityReturn = $this.find(".txtQuantityReturn ").val();
            item.SerialNoReturn = $this.find('.txtSerialNoReturn').val();

            item.QuantityOnHand = $this.data("quantity-on-hand");
            item.Code = $this.find('.lblCode').text();;

            
            lstItem.push(item);
        });

        st.Items = lstItem;

        var err = Validate(st);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/StockTransfer/Save",
            type: "POST",
            data: JSON.stringify(st),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                window.location.href = "/StockTransfer/Index";
            }
        });
    });


    $('#StockTransfer_CompanyTo').change(function () {
        var CompanyId = $(this).val();
        $.ajax({
            url: "/StockTransfer/GetById",
            type: "POST",
            data: "{'CompanyID' : '" + CompanyId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                var markup = "";
                for (var x = 0; x < data.length; x++) {
                    markup += "<option value=" + data[x].VehicleId + ">" + data[x].VehiclePlateNo + "</option>";
                }
                $("#UnitPlateNo").html(markup).show();
            }
        });
    });

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var itemId = $('#Items').val();

        var url = $(this).data('url') + '?itemId=' + itemId;
        $.get(url, function (data) {
            $('#tblItems').append(data);
        });
    });

    $(document).on('click', '.btnRemoveItem', function () {
        var $row = $(this).closest("tr");
        $row.fadeOut("fast", function () {
            $row.remove();
        })
    });

    $(document).on('keyup', '.txtQuantity', function () {
        var $row = $(this).closest("tr");
        var txtQuantityReturn = $row.find('.txtQuantityReturn');

        txtQuantityReturn.val($(this).val());
    });
});