
// MFC_TESTView.cpp: CMFCTESTView 類別的實作
//

#include "pch.h"
#include "framework.h"
// SHARED_HANDLERS 可以定義在實作預覽、縮圖和搜尋篩選條件處理常式的
// ATL 專案中，並允許與該專案共用文件程式碼。
#ifndef SHARED_HANDLERS
#include "MFC_TEST.h"
#endif

#include "MFC_TESTDoc.h"
#include "MFC_TESTView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCTESTView

IMPLEMENT_DYNCREATE(CMFCTESTView, CView)

BEGIN_MESSAGE_MAP(CMFCTESTView, CView)
	// 標準列印命令
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CMFCTESTView::OnFilePrintPreview)
	ON_WM_CONTEXTMENU()
	ON_WM_RBUTTONUP()
END_MESSAGE_MAP()

// CMFCTESTView 建構/解構

CMFCTESTView::CMFCTESTView() noexcept
{
	// TODO: 在此加入建構程式碼

}

CMFCTESTView::~CMFCTESTView()
{
}

BOOL CMFCTESTView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: 在此經由修改 CREATESTRUCT cs 
	// 達到修改視窗類別或樣式的目的

	return CView::PreCreateWindow(cs);
}

// CMFCTESTView 繪圖

void CMFCTESTView::OnDraw(CDC* /*pDC*/)
{
	CMFCTESTDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: 在此加入原生資料的描繪程式碼
}


// CMFCTESTView 列印


void CMFCTESTView::OnFilePrintPreview()
{
#ifndef SHARED_HANDLERS
	AFXPrintPreview(this);
#endif
}

BOOL CMFCTESTView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// 預設的準備列印程式碼
	return DoPreparePrinting(pInfo);
}

void CMFCTESTView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 加入列印前額外的初始設定
}

void CMFCTESTView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 加入列印後的清除程式碼
}

void CMFCTESTView::OnRButtonUp(UINT /* nFlags */, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void CMFCTESTView::OnContextMenu(CWnd* /* pWnd */, CPoint point)
{
#ifndef SHARED_HANDLERS
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
#endif
}


// CMFCTESTView 診斷

#ifdef _DEBUG
void CMFCTESTView::AssertValid() const
{
	CView::AssertValid();
}

void CMFCTESTView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CMFCTESTDoc* CMFCTESTView::GetDocument() const // 內嵌非偵錯版本
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CMFCTESTDoc)));
	return (CMFCTESTDoc*)m_pDocument;
}
#endif //_DEBUG


// CMFCTESTView 訊息處理常式
