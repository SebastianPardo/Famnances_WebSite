$(document).ready(function () {
    $(".period-card").on("click", function () {
        $("#periodId").val(this.id);
        $(".period-card").removeClass("active");
        $(this).addClass("active");
    });
});
