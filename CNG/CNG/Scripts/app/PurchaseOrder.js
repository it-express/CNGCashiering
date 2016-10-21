$(document).ready(function () {
    function Validate(po) {
        var err = "";

        if (po.VendorId == "") {
            err = "Vendor is required.";
        }
        else if (po.ShipTo == "") {
            err = "Ship To is required.";
        }
        else if (po.Items.length == 0) {
            err = "Please select item/s.";
        }

        return err;
    }

    $('#btnSubmit').click(function (event) {
        event.preventDefault();
        
        var purchaseOrder = new Object();
        purchaseOrder.No = $('#lblPoNumber').text();
        purchaseOrder.VendorId = $('#VendorId').val();
        purchaseOrder.ShipTo = $('#ShipTo').val();
        //purchaseOrder.Terms = (backen generated)
        //purchaseOrder.PreparedBy = (backend generated);
        //purchaseOrder.ApprovedBy = (backend generated);
        purchaseOrder.CheckedBy = $('#CheckedBy').val();

        purchaseOrder.Items = GetSelectedItems();

        var err = Validate(purchaseOrder);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/PurchaseOrder/Save",
            type: "POST",
            data: JSON.stringify(purchaseOrder),
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                alert("Successfully saved Purchase Order");

                var companyId = getParameterByName("companyId");
                window.location.href = "/PurchaseOrder/Index?companyId=" + companyId;
            }
        });
    });

    $('#VendorId').change(function () {
        var vendorId = $(this).val();

        $.ajax({
            url: "/Vendor/GetById",
            type: "POST",
            data: "{'id' : '" + vendorId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                var vendor = r;

                $('#lblVendorName').text(vendor.Name);
                $('#lblVendorAddress').text(vendor.Address);
                $('#lblVendorContactPerson').text(vendor.ContactPerson);
                $('#lblVendorContactNo').text(vendor.ContactNo);

                $('#lblTerms').text(vendor.Terms + ' days');

                var today = new Date();
                var dueDate = moment(today).add('days', vendor.Terms);

                $('#lblDueDate').text(dueDate.format("MMMM Do YYYY"));
            }
        });
    });

    function addDays(date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    $('#ShipTo').change(function () {
        var companyId = $(this).val();

        $.ajax({
            url: "/Company/GetById",
            type: "POST",
            data: "{'id' : '" + companyId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                var company = r;

                $('#lblCompanyname').text(company.Name);
                $('#lblShippingAddress').text(company.Address);
                $('#lblShippingContactPerson').text(company.ContactPerson);
                $('#lblShippingContactNo').text(company.ContactNo);
            }
        });
    });

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var itemId = $('#Items').val();

        $.ajax({
            url: "/Item/GetById",
            type: "POST",
            data: "{'id' : '" + itemId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                var item = r;
                //$('#tblItems tbody').empty();

                var result = "";
                result += "<tr class='item-row' data-item-id=" + item.Id + ">";
                result += "<td>" + item.Code + "</td>";
                result += "<td>" + item.Description + "</td>";
                result += "<td>" + item.Type.Description + "</td>";
                result += "<td>" + item.Brand + "</td>";
                result += "<td> <input type='number' class='txtQuantity form-control'> </td>";
                result += "<td>" + FormatNumber(item.UnitCost) + "</td>";
                result += "<td> <textarea class='txtRemarks' class='form-control'> </textarea></td>";
                result += "<td> <a href='#!' data-original-title='Remove' data-placement='top' class='btn btn-xs btn-red tooltips btnRemoveItem'><i class='fa fa-times fa fa-white'></i></a> </td>";
                result += "</tr>";

                $('#tblItems').append(result);
            }
        });
    });

    $(document).on('click', '.btnRemoveItem', function () {
        var $row = $(this).closest("tr");
        $row.fadeOut("fast", function () {
            $row.remove();
        })
    });

    function GetSelectedItems() {
        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.Id = $this.data("item-id");
            item.Quantity = $this.find(".txtQuantity").val();
            item.Remarks = $this.find(".txtRemarks").val();
            lstItem.push(item);
        });

        return lstItem;
    }

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }
});